<?php
    header('Content-type: application/json');
//    require("llconvertor/gpointconverter.class.php");
//    require("llconvertor/make64.php");
    require("parseData.php");
//    $convertor = new GpointConverter();
//    $utm = $convertor->convertLatLngToUtm(10, 10);
    
    $raw =  file_get_contents("php://input");
	$data = json_decode($raw);
	$result = parse($data);
    $resultJson = json_encode($result);
    
    echo $resultJson;
?>