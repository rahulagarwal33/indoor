<?php 
require("class_neuralnetwork.php");
class NNLoader
{
	protected $nn;
	protected $macList;
	public function getPosition($latId, $lonId, $jsonFP)
	{
		$input = $this->convertFPToSample($jsonFP);
		$output = $this->nn->calculate($input);
		$gridW = 50;
		$gridH = 50;
		$result[0] = ($latId + $output[0]) * $gridH / (60 * 1852.3);
		$result[1] = 0;
		$result[2] = ($lonId + $output[1]) * $gridW / (60 * 1852.3);
		return $result;
	}
	function convertFPToSample($jsonFP)
	{
		$input = array_fill(0, count($this->macList), 0);
		foreach($jsonFP as $fp)
		{
			if(isset($this->macList[$fp->mac]))
				$input[$this->macList[$fp->mac]] = $this->normalizeRSSI($fp->rssi);
			else
			{
				//mac is not registered
			}
		}
		return $input;
	}
	public function extractIO($sample)
	{
		$lat = $sample[0]["lat"];
		$lon = $sample[0]["lon"];
		$sampleMacList = array_column($sample, "mac");
		$sampleRSSIList = array_column($sample, "rssi");
		foreach($sampleMacList as $mac)
		{
			if(!isset($this->macList[$mac]))
				$this->macList[$mac] = count($this->macList);
		}
		$output = $this->normalize($lat, $lon);
		$input = array_fill(0, count($this->macList), 0);
		$cnt = count($sampleMacList);
		for($i = 0; $i < $cnt; ++$i)
		{
			$input[$this->macList[$sampleMacList[$i]]] = $this->normalizeRSSI($sampleRSSIList[$i]);
		}
		$result["input"] = $input;
		$result["output"] = $output;
		return $result;
	}
	public function train($sample)
	{
		$io = $this->extractIO($sample);
		$input = $io["input"];
		$output = $io["output"];
		if(!isset($this->nn))
		{
			$this->nn = new NeuralNetwork(count($input), count($input), count($output));
		}
		$ret = $this->nn->modifyLayerSize(0, count($this->macList));
		$ret = $ret | $this->nn->modifyLayerSize(1, count($this->macList));
		if($ret)
		{
			$this->nn->initWeights(true);
		}
		$this->nn->trainStep($input, $output);			
	}
	public function error($sample)
	{
		$io = $this->extractIO($sample);
		$input = $io["input"];
		$output = $io["output"];
		$err = $this->nn->squaredError($input, $output);
		return sqrt($err);
	}
	public function normalizeRSSI($rssi)
	{
		$rssi = ($rssi + 100) / 100;
		return $rssi;
	}
	public function normalize($lat, $lon)
	{
		$gridW = 50;
		$gridH = 50;
		$latM = $lat * 60 * 1852.3;
		$lonM = $lon * 60 * 1852.3;
		$result[0] = fmod($latM, $gridH) / $gridH;
		$result[1] = fmod($lonM, $gridW) / $gridW;
		return $result;
	}
}
?>