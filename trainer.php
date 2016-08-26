<?php
require("db.php");
require("nn/nn.php");
require("array_column.php");
function train($ll_ref)
{
	$db = new DB();
	$queryFindNN = "SELECT `nn_data` FROM `nn` WHERE `ll_ref` = " . $db->quote($ll_ref);
	$result = $db->select($queryFindNN);
	if(count($result) == 1)
	{
		$nnLoader = unserialize($result[0]["nn_data"]);
	}
	else
	{
		$nnLoader = new NNLoader();
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

		$prevPrecision = ini_get('serialize_precision');
		ini_set('serialize_precision', 10);
		$savedNetwork = serialize($nnLoader);
		ini_set('serialize_precision', $prevPrecision);
		$queryInsertNN = "INSERT INTO `nn` (`ll_ref`, `nn_data`, `error`) VALUES (" . $db->quote($ll_ref) . "," . $db->quote($savedNetwork) . "," . $db->quote($err) . ") ON DUPLICATE KEY UPDATE `nn_data` = VALUES(`nn_data`), `error` = VALUES(`error`)";
		$result= $db->query($queryInsertNN);
		var_dump($result);
	}
}
train(-2133747514);
function adapt($ll_ref, $db)
{

}
?>