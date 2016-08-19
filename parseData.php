<?php 
require("db.php");
require("nn/nn.php");
$gridW = 50;
$gridH = 50;
function parse($jsonFP)
{
	$num = count($jsonFP);
	$queryInsertData = "INSERT INTO `data` (`id`, `mac`, `rssi`, `lat`, `lon`, `ll_ref`) VALUES ";
	$queryInsertRouter = "INSERT INTO `routers` (`ll_ref`, `mac`, `model`) VALUES ";
	$cnt = 0;
	$db = new Db();
	foreach($jsonFP as $fp)
	{
		$llref = getLLRef($fp->lat, $fp->lon, $gridW, $gridH);
		$foundMac = getMacList($llref, $db);
		if($cnt != 0)
		{
			$queryInsertData = $queryInsertData . ", (" . $db->quote($fp->sampleID). "," . $db->quote($fp->mac) . "," . $db->quote($fp->fp) . "," . $db->quote($fp->lat) . "," . $db->quote($fp->lon) . "," . $db->quote($ll_ref) . ")";
			$queryInsertRouter = $queryInsertRouter . ", (" . $db->quote($llref) . "," . $db->quote($fp->mac) . "," . $db->quote($fp->model) . ")";
		}
		else
		{
			$queryInsertData = $queryInsertData . "(" . $db->quote($fp->sampleID). "," . $db->quote($fp->mac) . "," . $db->quote($fp->fp) . "," . $db->quote($fp->lat) . "," . $db->quote($fp->lon) . "," . $db->quote($ll_ref) . ")";
			$queryInsertRouter = $queryInsertRouter . "(" . $db->quote($llref) . "," . $db->quote($fp->mac) . "," . $db->quote($fp->model) . ")";
		}
		$cnt = $cnt + 1;
	}
	if($cnt != 0)
	{
		$queryInsertRouter = $queryInsertRouter . " ON DUPLICATE KEY UPDATE `model` = `model`";
		$result1 = $db->query($queryInsertData);
		if($result1 == false)
		{
			echo "<br> error result1";
			$result1 = $db->error();
		}
		$result2 = $db->query($queryInsertRouter);
		if($result2 == false)
		{
			echo "<br> error result2";
			$result2 = $db->error();
		}
		echo "<br> result1<br>";
		var_dump($result1);
		echo "<br> result2<br>";
		var_dump($result2);
	}
}
function getMacList($ll_ref, $db)
{
	$queryFindMac = "SELECT `mac` FROM `routers` WHERE `ll_ref` = " . $db->quote($ll_ref);
	$result = $db->select($queryFindMac);
	return $result;
}
function getPosition($jsonFP, $db)
{
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
			$llrefLst = array_merge($llrefLst, $result);
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
			$nnLoader = new NNLoader();
			$nnLoader->load($result[0]);
			$pos = $nnLoader->getPosition($jsonFP);
			for($i = 0; $i < 3; ++$i)
			{
				$finalPos[$i] += $pos[$i] * $occurance;	//for weighted averaging 
			}
			$occuranceCnt += $occurance;
		}
		else
		{
			//nerual network has not been created yet
			
		}
	}
	for($i = 0; $i < 3; ++$i)
	{
		$finalPos[$i] /= $occuranceCnt;
	}
	return $finalPos;
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
	return $id;
}
?>