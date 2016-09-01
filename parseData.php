<?php 
require("db.php");
require("nn/nn.php");
require("array_column.php");
function parse($jsonFP)
{
	$gridW = 50;
	$gridH = 50;
	$num = count($jsonFP);
	$queryInsertData = "INSERT INTO `data` (`id`, `mac`, `rssi`, `lat`, `lon`, `ll_ref`) VALUES ";
	$queryInsertRouter = "INSERT INTO `routers` (`ll_ref`, `mac`, `model`) VALUES ";
	$queryInsertLLRef = "INSERT INTO `llref` (`ll_ref`, `latId`, `lonId`) VALUES ";
	$cnt = 0;
	$db = new Db();
	foreach($jsonFP as $fp)
	{
		$llrefRet = getLLRef($fp->lat, $fp->lon, $gridW, $gridH);
		$llref = $llrefRet[0];
		$foundMac = getMacList($llref, $db);
		if($cnt != 0)
		{
			$queryInsertData = $queryInsertData . ", (" . $db->quote($fp->sampleID). "," . $db->quote($fp->mac) . "," . $db->quote($fp->rssi) . "," . $db->quote($fp->lat) . "," . $db->quote($fp->lon) . "," . $db->quote($llref) . ")";
			$queryInsertRouter = $queryInsertRouter . ", (" . $db->quote($llref) . "," . $db->quote($fp->mac) . "," . $db->quote($fp->model) . ")";
			$queryInsertLLRef = $queryInsertLLRef . ", (" . $db->quote($llref) . "," . $db->quote($llrefRet[1]) . "," . $db->quote($llrefRet[2]) . ")";
		}
		else
		{
			$queryInsertData = $queryInsertData . "(" . $db->quote($fp->sampleID). "," . $db->quote($fp->mac) . "," . $db->quote($fp->rssi) . "," . $db->quote($fp->lat) . "," . $db->quote($fp->lon) . "," . $db->quote($llref) . ")";
			$queryInsertRouter = $queryInsertRouter . "(" . $db->quote($llref) . "," . $db->quote($fp->mac) . "," . $db->quote($fp->model) . ")";
			$queryInsertLLRef = $queryInsertLLRef . "(" . $db->quote($llref) . "," . $db->quote($llrefRet[1]) . "," . $db->quote($llrefRet[2]) . ")";
		}
		$cnt = $cnt + 1;
	}
	if($cnt != 0)
	{
		$queryInsertRouter = $queryInsertRouter . " ON DUPLICATE KEY UPDATE `model` = `model`";
		$result1 = $db->query($queryInsertData);
		if($result1 == false)
		{
			$result1 = $db->error();
		}
		$result2 = $db->query($queryInsertRouter);
		if($result2 == false)
		{
			$result2 = $db->error();
		}
		$queryInsertLLRef = $queryInsertLLRef . " ON DUPLICATE KEY UPDATE `latId` = `latId`";
		$result3 = $db->query($queryInsertLLRef);
		if($result3 == false)
		{
			$result3 = $db->error();
		}
	}

	$result = array();
	$result["InsertRouter"] = $result1;
	$result["InsertData"] = $result2;
	$result["InsertLLRef"] = $result3;
	return $result;
}
function getMacList($ll_ref, $db)
{
	$queryFindMac = "SELECT `mac` FROM `routers` WHERE `ll_ref` = " . $db->quote($ll_ref);
	$result = $db->select($queryFindMac);
	return $result;
}
function getPosition($jsonFP)
{
	$db = new Db();
	$llrefLst = array();
	foreach($jsonFP as $fp)
	{
		$queryFindLL_Ref = "SELECT `ll_ref` FROM `routers` WHERE `mac` = " . $db->quote($fp->mac);
		$result = $db->select($queryFindLL_Ref);
		if(count($result) == 0)
		{
			//new mac found... 
		}
		else
		{
			$llrefLst = array_merge($llrefLst,  array_column($result, "ll_ref"));
		}
	}
	$llrefOccurance = array_count_values($llrefLst);
	$occuranceCnt = 0;
	$finalPos = array_fill(0, 3, 0);
	foreach($llrefOccurance as $llref => $occurance)
	{
		$queryFindNN = "SELECT `nn_data` FROM `nn` WHERE `ll_ref` = " . $db->quote($llref);
		$result = $db->select($queryFindNN);
		if(count($result) == 1)
		{
			$nnLoader = unserialize($result[0]["nn_data"]);
			$queryFindLLID = "SELECT * FROM `llref` WHERE `ll_ref` = " . $db->quote($llref);
			$resLLID = $db->select($queryFindLLID);
			if(count($resLLID) == 1)
			{
				$pos = $nnLoader->getPosition($resLLID[0]["latId"], $resLLID[0]["lonId"], $jsonFP);
				for($i = 0; $i < 3; ++$i)
				{
					$finalPos[$i] += $pos[$i] * $occurance;	//for weighted averaging 
				}
				$occuranceCnt += $occurance;
			}
		}
		else
		{
			//nerual network has not been created yet
			
		}
	}
	$accuracy = 10000;
	if($occuranceCnt != 0)
	{
		for($i = 0; $i < 3; ++$i)
		{
			$finalPos[$i] /= $occuranceCnt;
		}
		$accuracy = 1;
	}	
	$result = array();
	$result["lat"] = $finalPos[0];
	$result["y"] = $finalPos[1]; 
	$result["lon"] = $finalPos[2];
	$result["accuracy"] = $accuracy;
	return $result;
}
function getLLRef($lat, $lon, $gridW, $gridH)
{
	$latM = $lat * 60 * 1852.3;
	$lonM = $lon * 60 * 1852.3;
	$latIndex = intval($latM / $gridH);
	$lonIndex = intval($lonM / $gridW);
	$latIdStr = str_pad(dechex($latIndex), 8, "0", STR_PAD_LEFT);
	$lonIdStr = str_pad(dechex($lonIndex), 8, "0", STR_PAD_LEFT);
	$id = crc32($latIdStr . $lonIdStr);
	$ret = array();
	$ret[0] = $id;
	$ret[1] = $latIndex;
	$ret[2] = $lonIndex;
	return $ret;
}
?>