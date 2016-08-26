<?php
	header('Content-type: application/json');
	require("parseData.php");

    $raw =  file_get_contents("php://input");
	$data = json_decode($raw);
	$posData = getPosition($data);
	$result = json_encode($posData);
	echo $result;
?>