<?php
session_start();
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
    <h1>Login Example With PHP Session</h1>

    <?php if(isset($_SESSION["username"])): ?>

        <h2>Hi <?= $_SESSION["username"] ?>!</h2>
        <h3><a href="./logout.php" id="logout">Logout</h3>

    <?php else: ?>

        <form id="testloginsession">
            <label for="name">Username</label>
            <input type="text" id="username" required>
            <br>
            <label for="password">Password&nbsp;</label>
            <input type="password" id="password" required>
            <br>
            <input type="submit" value="Login">
            <br>
            <span id="message"></span>
        </form>
        <script type="text/javascript" src="login_session.js"></script>

    <?php endif; ?>
    
</body>
</html>