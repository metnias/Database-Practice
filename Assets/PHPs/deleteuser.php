<?php

$servername = "localhost";
$username = "root";
$password = "q1w2e3r4";
$dbname = "db_login";

$loginUser = $_POST["loginUser"];

$conn = new mysqli($servername, $username, $password, $dbname);

if($conn->connect_error) {
	die ("connection failed: " . $conn->connect_error);
}

$sql = "SELECT * FROM `tb_login` WHERE id = '".$loginUser."'";
$result = $conn -> query($sql);

if($result -> num_rows > 0){
	$deactivate_sql = "UPDATE `tb_login` SET `active` = '0' WHERE `id` = '".$loginUser."'";
	$conn -> query($deactivate_sql);
	$now = new DateTime("now");
	$updatedate_sql = "UPDATE `tb_login` SET `date` = '".$now -> format("c")."' WHERE `id` = '".$loginUser."'";
	$conn -> query($updatedate_sql);
	echo "SUCCESS_DELETE";
}
else{
	echo "ERROR_WRONG_ID";
}

$conn -> close();
