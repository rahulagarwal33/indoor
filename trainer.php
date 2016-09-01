<?php
require("db.php");
require("nn/nn.php");
require("array_column.php");

/**
 * Flush output buffer
 */
function myFlush() {
    echo(str_repeat(' ', 256));
    if (@ob_get_contents()) {
        @ob_end_flush();
    }
    flush();
}
function exceptions_error_handler($severity, $message, $filename, $lineno)
{
	if(error_reporting() == 0)
	{
		return;
	}
	else if($severity)
	{
		throw new ErrorException($message, 0, $severity, $filename, $lineno);
	}
}
set_error_handler('exceptions_error_handler');
//train(251574051);
trainAll();
function trainAll()
{
	$db = new DB();
	for($i = 0; $i < 10000; ++$i)
	{
		set_time_limit(300);
		echo "<br/>Iteration " . $i . "\n";
		$querySelectRandom = "SELECT `ll_ref` FROM `llref` ORDER BY RAND() LIMIT 1";
		$result = $db->select($querySelectRandom);
		if(count($result) == 1)
		{
			$ll_ref = $result[0]["ll_ref"];
			echo "<br/>Training network " . $ll_ref . "\n";
				train($ll_ref);
		}
		myFlush();
		sleep(1);
	}
}
function train($ll_ref)
{
	$db = new DB();
	$queryFindNN = "SELECT * FROM `nn` WHERE `ll_ref` = " . $db->quote($ll_ref);
	$result = $db->select($queryFindNN);
	if(count($result) == 1)
	{
		try
		{
			$nnLoader = unserialize($result[0]["nn_data"]);
			$epoch = $result[0]["epoch"];
		}
		catch(Exception $e)
		{
			$szData = strlen($result[0]["nn_data"]);
			$nnLoader = new NNLoader();
			$epoch = 0;
			echo "<br/>" . $e->getMessage() . " data size: " . $szData . "\n";
		}
	}
	else
	{
		$nnLoader = new NNLoader();
		$epoch = 0;
	}
	$queryFindData = "SELECT DISTINCT(`id`) FROM `data` WHERE `ll_ref` = " . $db->quote($ll_ref);
	$result = $db->select($queryFindData);
	if($result != null && count($result) > 0)
	{
		for ($i = 0; $i < count($result); $i ++)
		{
			$idx = mt_rand(0, count($result) - 1);
			$sampleID = $result[$idx];
			$querySampleData = "SELECT * FROM `data` WHERE `id` = " . $db->quote($sampleID["id"]);
			$samples = $db->select($querySampleData);
			if($samples != null && count($samples) > 0)
			{
				$nnLoader->train($samples);
			}
		}
		$idx = mt_rand(0, count($result) - 1);
		$sampleID = $result[$idx];
		$querySampleData = "SELECT * FROM `data` WHERE `id` = " . $db->quote($sampleID["id"]);
		$samples = $db->select($querySampleData);
		$err = 0;
		if($samples != null && count($samples) > 0)
			$err = $nnLoader->error($samples);

		echo "<br/> error: ". $err;
		$prevPrecision = ini_get('serialize_precision');
		ini_set('serialize_precision', 10);
		$savedNetwork = serialize($nnLoader);
		ini_set('serialize_precision', $prevPrecision);
		$sz = strlen($savedNetwork);
		$queryInsertNN = "INSERT INTO `nn` (`ll_ref`, `nn_data`, `nn_data_size`, `error`) VALUES (" . $db->quote($ll_ref) . "," . $db->quote($savedNetwork) . "," . $db->quote($sz) . "," . $db->quote($err) . ") ON DUPLICATE KEY UPDATE `nn_data` = VALUES(`nn_data`),`nn_data_size` = VALUES(`nn_data_size`),  `error` = VALUES(`error`), `epoch` = (`epoch` + 1), `ts` = now()";
		$result= $db->query($queryInsertNN);
		var_dump($result);
	}
}

function adapt($ll_ref, $db)
{

}

?>