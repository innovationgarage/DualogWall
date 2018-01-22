<?php
$ip = '93.188.234.';
if (!(substr($_SERVER['REMOTE_ADDR'], 0, strlen($ip)) === $ip)) {
    // deny access
    die('Sorry. Not allowed');
}
?>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Dualog Wall</title>
    <script src="js/jquery-1.11.3.min.js"></script>
    <link href="js/imageviewer.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        html,body {
            margin: 0;
            height: 100%;
        }

        body {
            background-color: black;
        }

        #wall {
            margin: 0;

            width: 100%;
            height: 100%;
            overflow: hidden;
        }
    </style>
    <script src="js/imageviewer.js"></script>
</head>

<body>
    <img src="wall.jpg" class="image" id="wall"></img>
    <script type="text/javascript">
        var viewer = ImageViewer(document.getElementById("wall"));
        $(".iv-container").css("display","block");
        $(".iv-container").height("100%");
    </script>
</body>

</html>