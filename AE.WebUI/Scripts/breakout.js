/// <reference path="_references.js" />
// Antti.E 2014

(function () {
    "use strict";
    var BO = {};
    BO = {

        /* settings */
        SPAWNTIME: 1000, /* time between spawn in ms*/
        MAXSPEED: 800, /* pixels per second */
        BGCOLOR: "#1E1E20",
        BLOCKCOLOR: "#374140",
        BALLCOLOR: "#DC3522",
        FONTCOLOR: "#D9CB9E",

        /* main init */
        init: function () {
            // set canvas size
            var canvas = document.getElementById('breakoutCanvas');
            // context to use in draws
            BO.C = canvas.getContext('2d');
            // game loop setup
            window.requestAnimationFrame = window.requestAnimationFrame || window.mozRequestAnimationFrame ||
                                          window.webkitRequestAnimationFrame || window.msRequestAnimationFrame;
            // for window size changes
            window.addEventListener('resize', BO.setupAnimation, false);
            // setup
            BO.setupAnimation();
            // loop
            requestAnimationFrame(BO.run);
        },

        /* setup animation */
        setupAnimation: function () {
            if (BO.height && BO.width) {
                var newHeight = document.body.clientHeight;
                var newWidth = document.body.clientWidth;
                // check do we really need to reset all
                if ((newHeight == BO.height) && (newWidth == BO.width)) {
                    return;
                }
            }
            // helper variables
            BO.animTime = Date.now();
            BO.height = document.body.clientHeight;
            BO.width = document.body.clientWidth;
            BO.spawnTimePassed = 0;
            BO.fps = 0;
            BO.frame = 0;
            BO.framesTime = 0;
            // canvas size
            BO.C.canvas.height = BO.height;
            BO.C.canvas.width = BO.width;
            // ball
            BO.Ball = {};
            BO.Ball.x = BO.width / 2;
            BO.Ball.y = BO.height - (BO.height / 2 / 2);
            BO.Ball.r = BO.maxSideSize() / 2;
            BO.Ball.xs = ((BO.MAXSPEED / 3) * Math.random()) + (BO.MAXSPEED / 3);
            BO.Ball.ys = ((BO.MAXSPEED / 3) * Math.random()) + (BO.MAXSPEED / 3);
            // blocks
            BO.Blocks = [];
            BO.Blocks.push(BO.newBlock());
        },

        // create new block
        newBlock: function () {
            // random size and position
            var halfMaxSize = BO.maxSideSize() / 2;
            var width = (Math.random() * halfMaxSize) + halfMaxSize;
            var height = (Math.random() * halfMaxSize) + halfMaxSize;
            var x = Math.random() * (BO.C.canvas.width - 2 * width);
            var y = Math.random() * (BO.C.canvas.height - 2 * height);
            // create block and return it
            return {
                width: width,
                height: height,
                x: x,
                y: y
            };
        },

        // block side size
        maxSideSize: function () {
            return (BO.C.canvas.width / 20);
        },

        // tick
        run: function () {
            BO.update();
            BO.draw();
            requestAnimationFrame(BO.run);
        },

        // update game logic
        update: function () {
            // time from last update
            var timePassed = Date.now() - BO.animTime;
            BO.animTime = Date.now();
            // fps calculation
            BO.frame++;
            if (BO.frame == 30) {
                BO.fps = 1000 / (BO.framesTime / 30);
                BO.frame = 0;
                BO.framesTime = 0;
            } else {
                BO.framesTime += timePassed;
            }
            // ball movement
            BO.Ball.x += (BO.Ball.xs * (timePassed / 1000));
            BO.Ball.y += (BO.Ball.ys * (timePassed / 1000));
            // because update can be called like minute later prevent
            // ball moving outside so we place it half out
            // so it will be kicked back in "faster".
            if (BO.Ball.x < (0 + BO.Ball.r)) {
                BO.Ball.x = BO.Ball.r;
            } else if (BO.Ball.x > (BO.width - BO.Ball.r)) {
                BO.Ball.x = BO.width - BO.Ball.r;
            }
            if (BO.Ball.y < (0 + BO.Ball.r)) {
                BO.Ball.y = BO.Ball.r;
            } else if (BO.Ball.y > (BO.height - BO.Ball.r)) {
                BO.Ball.y = BO.height - BO.Ball.r;
            }
            // new block ?
            BO.spawnTimePassed += timePassed;
            if (BO.spawnTimePassed > BO.SPAWNTIME) {
                BO.Blocks.push(BO.newBlock());
                BO.spawnTimePassed = 0;
            }
            // wall collisions
            if ((BO.Ball.x + BO.Ball.r) >= BO.width
                || (BO.Ball.x - BO.Ball.r) <= 0) {
                BO.Ball.xs *= -1;
            }
            if ((BO.Ball.y + BO.Ball.r) >= BO.height
                || (BO.Ball.y - BO.Ball.r) <= 0) {
                BO.Ball.ys *= -1;
            }
            // block collisions, rude simple check, at end replacing list with boxes
            // that were not hit
            var newBlocks = [];
            for (var i = 0 ; i < BO.Blocks.length ; i++) {
                // easy detection, treating ball as square, first x axis
                var ballX = BO.Ball.x - BO.Ball.r;
                var ballY = BO.Ball.y - BO.Ball.r;
                var ballSide = BO.Ball.r * 2;
                // Bx < Rx2 && Bx2 > Rx1 && By < Ry2 && By2 > Ry
                if (ballX < (BO.Blocks[i].x + BO.Blocks[i].width) &&
                    (ballX + ballSide) > BO.Blocks[i].x &&
                    ballY < (BO.Blocks[i].y + BO.Blocks[i].width) &&
                    (ballY + ballSide) > BO.Blocks[i].y) {
                    // hit on rectangle
                    var xSizeFactor = BO.Blocks[i].width / BO.maxSideSize();
                    var ySizeFactor = BO.Blocks[i].height / BO.maxSideSize();
                    if (BO.Ball.xs < 0) {
                        BO.Ball.xs = xSizeFactor * BO.MAXSPEED;
                    } else {
                        BO.Ball.xs = xSizeFactor * BO.MAXSPEED * -1;
                    }
                    if (BO.Ball.ys < 0) {
                        BO.Ball.ys = ySizeFactor * BO.MAXSPEED;
                    } else {
                        BO.Ball.ys = ySizeFactor * BO.MAXSPEED * -1;
                    }

                }
                else {
                    // no hit
                    newBlocks.push(BO.Blocks[i]);
                }
            }
            BO.Blocks = newBlocks;
        },

        // draw canvas
        draw: function () {
            BO.C.fillStyle = BO.BGCOLOR;
            BO.C.fillRect(0, 0, BO.C.canvas.width, BO.C.canvas.height);
            // draw ball
            BO.C.beginPath();
            BO.C.fillStyle = BO.BALLCOLOR;
            BO.C.arc(BO.Ball.x, BO.Ball.y, BO.Ball.r, 0, 2 * Math.PI);
            BO.C.fill();
            // blocks
            for (var i = 0 ; i < BO.Blocks.length ; i++) {
                BO.C.fillStyle = BO.BLOCKCOLOR;
                BO.C.fillRect(BO.Blocks[i].x, BO.Blocks[i].y, BO.Blocks[i].width, BO.Blocks[i].height);
            }
            // hud
            BO.C.fillStyle = BO.FONTCOLOR;
            BO.C.font = 'Monospace';
            BO.C.fillText('FPS: ' + Math.round(BO.fps), 20, 20);
        }

    }; /* BO end*/

    /* automatically launch animation */
    BO.init();
})();
