<?php
session_start();
$json = file_get_contents('php://input');
$b = json_decode($json);

header("Content-Type: application/json; charset=UTF-8");
header("Access-Control-Allow-Origin: http://192.168.1.52"); // -> CORS Policy
header("Access-Control-Allow-Methods: POST");
header("Access-Control-Allow-Headers: Content-Type");

try {
    if($b->username == "Fabrizio" && $b->password == "Fabrizio123")
    {
        // NOTE: session is only available for the local server. 
        // If you want to develop for example a web app with Rest API, a cool solution may be for example JWT system.
        // Alternatively you can for example set the client session_id (for example from cookie) to the session_id($your_id) befor session_start().
        $_SESSION["username"] = $b->username;
        echo json_encode(array('Result' => 'ok'));
    }
    else {
        echo json_encode(array('Result' => 'Error'));
    }
}
catch (Exception $e) {
    echo json_encode(array('Result' => 'Error'));
}
?>