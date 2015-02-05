/// <reference path="_references.js" />
// Antti.E 2014

/**
 * direction helper
 */
var Dir = {
    /**
     * enumeration
     */
    N: 1,
    E: 2,
    S: 3,
    W: 4,
    /**
     * random direction
     */
    random: function () {
        var d = Math.floor(Math.random() * 4) + 1;
        return d;
    },
    /**
     * random directions
     * http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
     * @return array of all directions in random order
     */
    randoms: function () {
        var dirs = [Dir.N, Dir.E, Dir.S, Dir.W];
        var back = dirs.length, temp, front;
        // count from backwards
        while (back) {
            // pick random from front
            front = Math.floor(Math.random() * back--);
            // save the one in back
            temp = dirs[back];
            // the random from front into back
            dirs[back] = dirs[front];
            // the one from back to front
            dirs[front] = temp;
        }
        return dirs;
    },
    /**
     * opposite direction
     */
    opposite: function (direction) {
        switch (direction) {
            case Dir.N:
                return Dir.S;
            case Dir.S:
                return Dir.N;
            case Dir.W:
                return Dir.E;
            case Dir.E:
                return Dir.W;
            default:
                throw "invalid direction";
        }
    }
};

/**
 * cell object
 * @class
 * @constructor
 */
function Cell() {
    // walls for North, East, South, West
    var N = false;
    this.getN = function () { return N; }
    var E = false;
    this.getE = function () { return E; }
    var S = false;
    this.getS = function () { return S; }
    var W = false;
    this.getW = function () { return W; }

    /**
     * helper to check has any modifications been done to this
     * @return {bool} true if any
     */
    this.isVisited = function () {
        if (N || E || S || W) {
            return true;
        }
        return false;
    }

    /**
     * open a side from cell
     * @param {number} direction / side
     * @return {bool} true if successful
     */
    this.open = function (direction) {
        switch (direction) {
            case Dir.N:
                if (!N) { N = true; return true; } else { return false; }
            case Dir.E:
                if (!E) { E = true; return true; } else { return false; }
            case Dir.S:
                if (!S) { S = true; return true; } else { return false; }
            case Dir.W:
                if (!W) { W = true; return true; } else { return false; }
        }
        return false;
    }

    /**
     * ask can you open a side of cell
     * @param {number} direction / side (can be null then opened sides tells you it)
     * @return {bool} true if you can
     */
    this.canOpen = function (direction) {
        if (!direction) {
            var o = 0;
            if (N) { o++; }
            if (E) { o++; }
            if (S) { o++; }
            if (W) { o++; }
            if (o >= 3) { return false; } else { return true; }
        }
        else {
            switch (direction) {
                case Dir.N:
                    if (!N) { return true; } else { return false; }
                case Dir.E:
                    if (!E) { return true; } else { return false; }
                case Dir.S:
                    if (!S) { return true; } else { return false; }
                case Dir.W:
                    if (!W) { return true; } else { return false; }
            }
        }
    }
};

/**
 * position helper, just like a pointer.
 * @class
 * @constructor
 * @param {number} xPos
 * @param {number} yPos
 */
function Pos(xPos, yPos) {
    var xp = typeof xPos !== 'undefined' ? xPos : 0;
    /**
     * get/set x
    */
    this.x = function (v) {
        if (!v) { return xp; }
        else { xp = v; }
    }
    var yp = typeof yPos !== 'undefined' ? yPos : 0;
    /**
     * get/set y
     */
    this.y = function (v) {
        if (!v) { return yp; }
        else { yp = v; }
    }
    /**
     * move point to given direction
     */
    this.move = function (direction) {
        switch (direction) {
            case Dir.N:
                yp--;
                break;
            case Dir.E:
                xp++;
                break;
            case Dir.S:
                yp++;
                break;
            case Dir.W:
                xp--;
                break;
        }
    }
    /**
     * create copy
     */
    this.copy = function () {
        var np = new Pos(xp, yp);
        return np;
    }
};

/**
 * maze generator
 * @class
 * @constructor
 * @param {number} horizontalSize (optional)
 * @param {number} verticalSize (optional)
 */
function Maze(horizontalSize, verticalSize) {

    // members
    var maze; // so mazy, wow much corners
    var path; // current path
    var cPos; // current position
    var sPos; // start position
    var ePos; // end position (farthest away from start)
    var xSize; // size horizontally
    var ySize; // size vertically
    var steps; // generation step count
    var longestPath; // helper to know what is the farthest path we found so far

    /******************* PRIVATE *******************/
    // init
    init(horizontalSize, verticalSize);

    // initialization
    function init(horizontalSize, verticalSize) {
        // size
        xSize = typeof horizontalSize !== 'undefined' ? horizontalSize : 10;
        ySize = typeof verticalSize !== 'undefined' ? verticalSize : 10;

        // maze array
        maze = new Array(xSize);
        for (var i = 0; i < maze.length; i++) {
            maze[i] = new Array(ySize);
        }

        // create cells into array
        for (var x = 0; x < maze.length; x++) {
            for (var y = 0; y < maze[x].length; y++) {
                maze[x][y] = new Cell();
            }
        }

        // choose starting point in the maze
        cPos = new Pos();
        cPos.x(Math.floor(Math.random() * xSize));
        cPos.y(Math.floor(Math.random() * ySize));

        steps = 0; // reset steps
        path = new Array();
        path.push(cPos);

        // start and end tracking
        sPos = cPos.copy();
        ePos = sPos.copy();
        longestPath = path.length;
    }

    // one step in generation
    function step() {
        if (path.length > 0) {
            if (!dig()) {
                // remove path / go back
                cPos = path.pop();
            }
            // maze is not finished until no path anymore
            // "we are back at start"
            return false;
        }
        // maze has been created
        return true;
    }

    // dig a new path
    function dig() {
        // check current
        if (!m(cPos).canOpen()) {
            return false;
        }

        // check where we can go
        var rdirs = Dir.randoms();
        for (var i = 0; i < rdirs.length; i++) {
            var dir = rdirs[i];
            var np = cPos.copy();
            np.move(dir);
            // is direction "valid" inside arrays
            if (np.x() >= xSize || np.x() < 0 ||
                np.y() >= ySize || np.y() < 0) {
                continue;
            }
            // have we visited it?
            if (m(np).isVisited()) {
                continue;
            }
            // can the next cell open wall to current?
            if (!m(np).canOpen(Dir.opposite(dir))) {
                continue;
            }
            /***** all seems ok to dig *****/
            // open wall in our current position
            m(cPos).open(dir);
            // open wall in next position towards current
            m(np).open(Dir.opposite(dir));
            // set new position
            cPos = np;
            // append into path
            path.push(cPos);
            // track end point
            if (path.length > longestPath) {
                ePos = cPos.copy();
                longestPath = path.length;
            }
            return true;
        }
        return false;
    }

    // helper for accessing maze cell in current position
    function m(pos) {
        return maze[pos.x()][pos.y()];
    }

    /******************* PUBLIC *******************/

    /**
     * reset maze completely
     * @param {number} horizontalSize (optional)
     * @param {number} verticalSize (optional)
     */
    this.reset = function (horizontalSize, verticalSize) {
        init(horizontalSize, verticalSize);
    }

    /**
     * is maze generation finished
     */
    this.isFinished = function () {
        if (steps != 0 && path.length == 0) {
            return true;
        }
        return false;
    }

    /**
     * completely generate maze or finish current
     */
    this.generate = function () {
        while (!step()) { }
    }

    /**
     * step forward with generation
     * @return {bool} true if finisned
     */
    this.generateStep = function () {
        return step();
    }

    /**
     * 2d maze array of cells
     * @return {Array} of Cell objects
     */
    this.getMaze = function () { return maze; }

    /**
     * current position under construction
     * @return {Pos}
     */
    this.getPos = function () { return cPos; }

    /**
     * starting position of maze
     * @return {Pos}
     */
    this.getStartPos = function () { return sPos; }

    /**
     * end position of maze (farthest away from start)
     * this changes during generation.
     * @return {Pos}
     */
    this.getEndPos = function () { return ePos; }

    /**
     * current path under construction
     * @return {Array} of Pos objects
     */
    this.getPath = function () { return path; }

    /******************* DEBUG *******************/
    function ascii() {
        var dp = "";
        for (var y = 0; y < ySize ; y++) {
            var line1 = ""; // ###
            var line2 = ""; // #?#
            var line3 = ""; // ###
            for (var x = 0; x < xSize; x++) {
                if (maze[x][y].getN()) { line1 += "# #"; } else { line1 += "###"; }
                if (maze[x][y].getW()) { line2 += " "; } else { line2 += "#"; }
                // start 
                if (x == sPos.x() && y == sPos.y()) {
                    line2 += "S";
                }
                    // current?
                else if (x == cPos.x() && y == cPos.y()) {
                    line2 += "+";
                }
                    // end
                else if (x == ePos.x() && y == ePos.y()) {
                    line2 += "E";
                }
                else {
                    line2 += " ";
                }
                if (maze[x][y].getE()) { line2 += " "; } else { line2 += "#"; }
                if (maze[x][y].getS()) { line3 += "# #"; } else { line3 += "###"; }
            }
            dp += line1 + "\n" + line2 + "\n" + line3 + "\n";
        }
        return dp;
    }
    this.ascii = function () { return ascii(); }
};

/**
 * using the maze generator on a canvas
 * @constructor
 * @param {canvas} canvas mandatory
 * @param {number} xSize size of maze horizontally (optional)
 * @param {number} ySize size of maze vertically (optional)
 */
function MazeCanvas(canvas, xSize, ySize) {
    /******************* PRIVATE ******************/
    var timeFrame; // time we want to spend in single frame
    var timeElapsed; // time we have spend on current frame
    var timeLast; // time of last update
    var paused; // is generation paused
    var maze; // much wow, so maze
    var ctx; // canvas context
    var w; // canvas width
    var h; // canvas height
    var cw; // cell width
    var ch; // cell height

    var colorUnvisited = 'darkgray';
    var colorGrid = 'lime';
    var colorVisited = 'ivory';
    var colorWalls = 'gray';
    var colorPath = 'orange';
    var colorCurrent = 'red';
    var colorStart = 'yellow';
    var colorEnd = 'green';

    // check input
    if (!canvas) { throw "canvas must be defined"; }
    xSize = typeof xSize !== 'undefined' ? xSize : 10;
    ySize = typeof ySize !== 'undefined' ? ySize : 10;

    // create maze object
    maze = new Maze(xSize, ySize);

    // default time whe spend on a single frame
    timeFrame = 1000;

    initialize(xSize, ySize);

    // initialize
    function initialize(xs, ys) {
        xSize = xs;
        ySize = ys;
        maze.reset(xSize, ySize); // maze reset
        ctx = canvas.getContext('2d'); // context to draw
        w = canvas.width;
        h = canvas.height;
        cw = Math.floor(w / xs);
        ch = Math.floor(h / ys);
    }

    // animation call back
    function animation() {
        if (isTime()) {
            update();
            draw();
        }
        loop();
    }

    // loop our animation
    function loop() {
        if (!paused) {
            requestAnimationFrame(animation);
        }
    }

    // is it time to update?
    function isTime() {
        timeLast = typeof timeLast !== 'undefined' ? timeLast : Date.now();
        if (Date.now() - timeLast > timeFrame) {
            timeLast = Date.now();
            return true;
        }
        return false;
    }

    // update
    function update() {
        if (maze.generateStep()) {
            paused = true;
        }
    }

    // draw maze
    function draw() {
        ctx.clearRect(0, 0, w, h);
        var m = maze.getMaze();
        var path = maze.getPath();
        var pos = maze.getPos();
        var ePos = maze.getEndPos();
        var sPos = maze.getStartPos();
        for (var x = 0; x < xSize ; x++) {
            for (var y = 0; y < ySize; y++) {

                var xc = x * cw; // cell x
                var yc = y * ch; // cell y

                // background
                if (m[x][y].isVisited()) {
                    ctx.fillStyle = colorVisited;
                }
                else {
                    ctx.fillStyle = colorUnvisited;
                }
                ctx.fillRect(xc, yc, cw, ch);
                // active path
                ctx.fillStyle = colorPath;
                for (var i = 0; i < path.length; i++) {
                    if (path[i].x() == x && path[i].y() == y) {
                        var radius = cw > ch ? cw : ch;
                        radius = radius / 6;
                        ctx.beginPath();
                        ctx.arc(xc + (cw / 2), yc + (ch / 2), radius, 0, Math.PI * 2);
                        ctx.fill();
                        break;
                    }
                }
                // walls
                ctx.fillStyle = colorWalls;
                var wt = ch / 5; // wall thickness
                // N
                if (!m[x][y].getN()) {
                    ctx.fillRect(xc, yc, cw, wt);
                }
                // S
                if (!m[x][y].getS()) {
                    ctx.fillRect(xc, yc + ch - wt, cw, wt);
                }
                // W
                if (!m[x][y].getW()) {
                    ctx.fillRect(xc, yc, wt, ch);
                }
                // E
                if (!m[x][y].getE()) {
                    ctx.fillRect(xc + cw - wt, yc, wt, ch);
                }
                // end;
                ctx.fillStyle = colorEnd;
                if (ePos.x() == x && ePos.y() == y) {
                    var radius = cw > ch ? cw : ch;
                    radius = radius / 4;
                    ctx.beginPath();
                    ctx.arc(xc + (cw / 2), yc + (ch / 2), radius, 0, Math.PI * 2);
                    ctx.fill();
                }
                // start
                ctx.fillStyle = colorStart;
                if (sPos.x() == x && sPos.y() == y) {
                    var radius = cw > ch ? cw : ch;
                    radius = radius / 4;
                    ctx.beginPath();
                    ctx.arc(xc + (cw / 2), yc + (ch / 2), radius, 0, Math.PI * 2);
                    ctx.fill();

                }
                // current pointer
                ctx.fillStyle = colorCurrent;
                if (pos.x() == x && pos.y() == y) {
                    var radius = cw > ch ? cw : ch;
                    radius = radius / 5;
                    ctx.beginPath();
                    ctx.arc(xc + (cw / 2), yc + (ch / 2), radius, 0, Math.PI * 2);
                    ctx.fill();
                }
            }
        }
    }

    /******************* PUBLIC *******************/

    /**
     * reset maze / resize it
     * @param {number} xSize size of maze horizontally (optional)
     * @param {number} ySize size of maze vertically (optional)
     */
    this.reset = function (xs, ys) {
        paused = true;
        initialize(xs, ys);
    }

    /**
     * start generation / play
     */
    this.play = function () {
        paused = false;
        loop();
    }

    /**
     * pause generation
     */
    this.pause = function () {
        paused = true;
    }

    /**
     * set time spend on each step of generation
     * @param {number} ms milli seconds
     */
    this.setTimeFrame = function (ms) {
        timeFrame = ms;
    }
};

/* running it part */
var c, w, h, cw, ch, m;

function runit() {
    w = c.width;
    h = c.height;
    cw = Math.floor(w / 30);
    ch = Math.floor(h / 30);
    m.reset(cw, ch);
    m.play();
};

$(document).ready(function () {
    // animation frame setup
    window.requestAnimationFrame = window.requestAnimationFrame || window.mozRequestAnimationFrame ||
                              window.webkitRequestAnimationFrame || window.msRequestAnimationFrame;
    // canvas
    d = $('#mazeDiv')[0];
    c = $('#mazeCanvas')[0];
    m = new MazeCanvas(c);
    m.setTimeFrame(100);
    window.addEventListener('resize', function (e) {
        m.pause();
        runit();
    }, false);
    runit();
});

