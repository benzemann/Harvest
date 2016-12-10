var gamePath = '';
var localVersion = '';
var gitVersion = '';

window.onload = function() {
	
	console.log("Initializing...");
	
	document.getElementById("play").style.visibility = "hidden";
	document.getElementById("download").style.visibility = "hidden";
	
	LoadGamePath();
	
	document.getElementById('download').onclick = function() {
		DownloadGame();
	}
	document.getElementById('pickFolder').onclick = function() {
		PickNewDirectory();
	}
	document.getElementById('play').onclick = function() {
		RunGame();
	}
}

function CheckForUpdates(){
	if(gamePath == ''){
		const electron = require('electron')
		const remote = electron.remote
		const mainProcess = remote.require('./main')
		var path = require('path');
		var appDir = path.dirname(require.main.filename);
		gamePath = appDir + "\\Game";
		document.getElementById("projectFolder").innerHTML = "Game folder: " + gamePath;
	}
	GetVersionFromGit();
}

function CompareLocaleAndGitVersion(){
	var fs = require('fs');
	
	if(fs.existsSync(gamePath + "\\version.txt")){
		document.getElementById("play").style.visibility = "visible";
	}
	if(localVersion != gitVersion && localVersion != ""){
		document.getElementById("download").style.visibility = "visible";
		document.getElementById("download").innerHTML = "Update game";
	} else if (localVersion == gitVersion && localVersion != ""){
		document.getElementById("download").style.visibility = "hidden";
	} else {
		document.getElementById("download").style.visibility = "visible";
	}
}

function DownloadGame(){
	
	document.getElementById("download").style.visibility = "hidden";
	
	var http = require('http');
	var request = require('request');
	var fs = require('fs');
	var url = "https://dl.dropboxusercontent.com/u/11766236/Harvest.zip";
	//var dest = "tmp.txt";
	var file = fs.createWriteStream(gamePath + "\\Harvest.zip");
	var result = '';
	var received_bytes = 0;
    var total_bytes = 0;
	request(url)
	.on('response', function ( data ) {
        // Change the total bytes value to get progress later.
        total_bytes = parseInt(data.headers['content-length' ]);
    })
	.on('data', function(chunk) {
        // Update the received bytes
        received_bytes += chunk.length;

        var percentage = (received_bytes * 100) / total_bytes;
		document.getElementById("downloadProgress").innerHTML = "Download progress: " + Math.floor(percentage) + "%";
    })
	.on('error', function(error){
		console.log(error);
	})
	.on('end', function () {
		document.getElementById("downloadProgress").innerHTML = "Download Completed ";
		UnzipGameFile();
	})
	.pipe(file);
	
}

function UnzipGameFile(){
	
	var DecompressZip = require('decompress-zip');
	
	var unzipper = new DecompressZip(gamePath + '\\Harvest.zip');
	
	unzipper.on('error', function (err) {
		console.log('Caught an error', err);
	});
	
	unzipper.on('extract', function (log) {
		document.getElementById("downloadProgress").innerHTML = "Extraction complete";
		var fs = require('fs');
		var filePath = gamePath + "\\Harvest.zip"; 
		fs.unlinkSync(filePath);
		LoadGamePath();
	});
	
	unzipper.on('progress', function (fileIndex, fileCount) {
		percentage = ((fileIndex + 1.0)/fileCount) * 100.0;
		document.getElementById("downloadProgress").innerHTML = "Extracting progress: " + Math.floor(percentage) + "%";
	});
	
	unzipper.extract({
		path: gamePath + ''
	});
	
	console.log("Done unzipping...");
	
}

function RunGame(){
	if(gamePath == '')
		return;
	
	if(process.platform == "win32"){
		var child = require('child_process').execFile;
	
		child(gamePath + "\\Harvest.exe", function(err, data) {
		if(err){
		   console.error(err);
		   return;
		}
	 
		console.log("Run game...");
	});
	} else {
		document.getElementById("downloadProgress").innerHTML = "Operating system not supported."
	}
	
}

function GetVersionFromGit(){
	var http = require('http');
	var request = require('request');
	//var fs = require('fs');
	var url = "https://dl.dropboxusercontent.com/u/11766236/Version.txt";
	//var dest = "tmp.txt";
	//var file = fs.createWriteStream(dest);
	var result = '';
	request(url)
	.on('data', function (chunk){ result += chunk; })
	.on('error', function(error){
		console.log(error);
		GetLocalVersion();
	})
	.on('end', function () {
		document.getElementById("version").innerHTML = "Version: " + result;
		gitVersion = result;
		GetLocalVersion();
	});
}

function GetLocalVersion(){
	if(gamePath == null){
		return;
	}
	var fs = require('fs');
	var path = gamePath + "\\version.txt";

	var data = fs.readFile(path, function (err, data){
		if (err) console.log("no version");
		if(data != null)
			localVersion = data;
		else
			localVersion = '';
		document.getElementById("localVersion").innerHTML = "Local version: " + localVersion;
		CompareLocaleAndGitVersion();
	});
}

function PickNewDirectory(){
	const electron = require('electron')
	const remote = electron.remote
	const mainProcess = remote.require('./main')
	var dir = mainProcess.selectDirectory()
	if(dir != null){
		document.getElementById("projectFolder").innerHTML = "Game folder: " + dir[0];
		gamePath = dir[0];
		SaveGamePath();
		GetLocalVersion();
	}
}

function LoadGamePath(){
	var fs = require('fs');
	if (fs.existsSync('gamePath.txt')) {
		var data = fs.readFileSync('gamePath.txt','utf8');
		gamePath = data;
	}
	document.getElementById("projectFolder").innerHTML = "Game folder: " + gamePath;
	CheckForUpdates()
}

function SaveGamePath(){
	var fs = require('fs');
	fs.writeFile('gamePath.txt', gamePath, function(err) { 
		if(err)
			console.log(err);
	});
}