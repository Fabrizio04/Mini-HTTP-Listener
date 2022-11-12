<?php

session_start();

//header("Location: index.html");
//http_response_code(404);

// if(!isset($_SESSION["prova"])){
//     $_SESSION["prova"] = "ciao";
// }

//session_destroy();

//print_r($_GET);
//echo $_GET["prova"];
//print_r($_SERVER);
//print_r($_COOKIE);
//header("Location: ./index.html");
echo (isset($_SERVER['HTTPS']) && $_SERVER['HTTPS'] === 'on' ? "https" : "http") . "://$_SERVER[REMOTE_ADDR]$_SERVER[REQUEST_URI]";

$json = file_get_contents('php://input');
$b = json_decode($json);
//echo $b->date;
?>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Document</title>
</head>
<body>
    <h1><?php echo "Hello World with PHP!"; ?></h1>
    <h1><?php if(isset($_SESSION["prova"])) echo $_SESSION["prova"]; ?></h1>
    <?php
    for($i = 0; $i < 10; $i++)
        echo '<p>Ciao '.($i+1).'</p>';
    ?>
</body>
</html>