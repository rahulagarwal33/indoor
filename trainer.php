<?php
function train($ll_ref, $db)
{
	$queryFindNN = "SELECT `nn_data` FROM `nn` WHERE `ll_ref` = " . $db->quote($ll_ref);
	$result = $db->select($queryFindNN);
	if(count($result) == 1)
	{
		$nnLoader = new NNLoader();
		$nnLoader->load($result[0]);
	}
	else
	{
		$queryFindData = "SELECT * FROM `data` WHERE `ll_ref` = " . $db->quote($ll_ref);
		$result = $db->select($queryFindData);
		$nnLoader = new NNLoader();
		$nnLoader->
	}
}
function adapt($ll_ref, $db)
{

}
?>