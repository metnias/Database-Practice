<?php

$servername = "localhost";
$username = "root";
$password = "q1w2e3r4";
$dbname = "db_login";

$loginUser = $_POST["loginUser"];
$loginPass = $_POST["loginPass"];

$conn = new mysqli($servername, $username, $password, $dbname);

if($conn->connect_error) {
	die ("connection failed: " . $conn->connect_error);
}

$sql = "SELECT * FROM `tb_login` WHERE `tb_login`.`id` = '".$loginUser."'";
$result = $conn -> query($sql);

if($result -> num_rows > 0){
	while ($row = $result -> fetch_assoc()){
		if($row["pw"] == $loginPass){
			if($row["active"] == '1'){
				echo "SUCCESS_LOGIN";
			}
			else{
				echo "ERROR_REMOVED";
			}
			$conn -> close();
			exit;
		}
	}
	echo "ERROR_WRONG_PW";
} else {
	echo "ERROR_WRONG_ID";
}

$conn -> close();
