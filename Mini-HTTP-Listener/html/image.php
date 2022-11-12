<?php

// // open the file in a binary mode
// $name = './image.jpg';
// $fp = fopen($name, 'rb');

// // send the right headers
// header("Content-Type: image/jpeg");
// header("Content-Length: " . filesize($name));

// // dump the picture and stop the script
// fpassthru($fp);
// exit;

$file = './image.jpg';

if (file_exists($file)) {
    //header('Content-Description: File Transfer');
    //header('Content-Disposition: attachment; filename="'.basename($file).'"'); // -> this 2 lines to force file download 
    header('Content-Type: image/jpeg');
    header('Expires: 0');
    header('Cache-Control: must-revalidate');
    header('Pragma: public');
    header('Content-Length: ' . filesize($file));
    readfile($file);
    exit;
}

?>