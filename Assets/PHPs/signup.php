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

$now = new DateTime("now");
if($result -> num_rows > 0){
	$row = $result -> fetch_assoc();
	if ($row["active"] == '1'){	echo "ERROR_ID_DUPE"; $conn -> close(); exit; }
	else {
		$date = new DateTime($row["date"]);
		$interval = $now -> diff($date, true);
		if ($interval -> i > 5)	{
			$remove_sql = "DELETE FROM `tb_login` WHERE `tb_login`.`id` = '".$loginUser."'";
			$conn -> query($remove_sql);
		}
		else{
			echo "ERROR_RECENT"; $conn -> close(); exit;
		}
	}
}

$insert_sql = "INSERT INTO `tb_login` (id, pw, date, active) VALUES ('".$loginUser."', '".$loginPass."', '".$now -> format("c")."', '1' );";
$conn -> query($insert_sql);
echo "SUCCESS_SIGNUP";

$conn -> close();
