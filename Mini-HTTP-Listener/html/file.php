<?php
echo "Filename: " . $_FILES['myFile']['name']."<br>";
echo "Type : " . $_FILES['myFile']['type'] ."<br>";
echo "Size : " . $_FILES['myFile']['size'] ."<br>";
echo "Temp name: " . $_FILES['myFile']['tmp_name'] ."<br>";
echo "Error : " . $_FILES['myFile']['error'] . "<br>";

$uploaddir = './tmp/';
$uploadfile = $uploaddir . basename($_FILES['myFile']['name']);

if(is_uploaded_file($_FILES['myFile']['tmp_name'])){

    if (move_uploaded_file($_FILES['myFile']['tmp_name'], $uploadfile))
        echo "File is valid, and was successfully uploaded.<br>";
}

echo "<br><br>";

echo "Filename: " . $_FILES['myFile2']['name']."<br>";
echo "Type : " . $_FILES['myFile2']['type'] ."<br>";
echo "Size : " . $_FILES['myFile2']['size'] ."<br>";
echo "Temp name: " . $_FILES['myFile2']['tmp_name'] ."<br>";
echo "Error : " . $_FILES['myFile2']['error'] . "<br>";

$uploaddir = './tmp/';
$uploadfile = $uploaddir . basename($_FILES['myFile2']['name']);

if(is_uploaded_file($_FILES['myFile2']['tmp_name'])){

    if (move_uploaded_file($_FILES['myFile2']['tmp_name'], $uploadfile))
        echo "File is valid, and was successfully uploaded.<br>";
}

echo "<br><br>";
foreach($_POST as $key => $value){
	echo $key." = ".$value."<br>";
}
?>