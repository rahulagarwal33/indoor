<?php 
require("class_neuralnetwork.php");
class NNLoader
{
	protected $nn;
	public function load($nn_data)
	{
		//load the network
		$nn = new NeuralNetwork();
		$nn_array = unserialize($nn_data);
		$nn->import($nn_array);
	}
	public function save()
	{
		$nn_array = $nn->export();
		$nnData = serialize($nn_array);
		return $nnData;
	}
	public function getPosition($jsonFP)
	{
		return array_fill(0, 3, 0);
	}
}
?>