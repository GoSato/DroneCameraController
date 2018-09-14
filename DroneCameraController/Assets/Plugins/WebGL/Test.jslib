var WebRecordPlugin = 
{

$WebRecord: {
	mediaRecorder: {},
	recordedBlobs: {},
	sourceBuffer: {},
	video: {},
	canvas: {},
	mediaSource: {},
	recordButton: {},
	playButton: {},
	downloadButton: {},
	canvasStream: {},
	audioContext: {},
	audioNode: {},
	destination: {},
	oscillator: {},
	audioStream: {},
	mediaStream: {},
	},

	LoadRecorder : function()
	{
		function handleSourceOpen(event) {
		  console.log('MediaSource opened');
		  WebRecord.sourceBuffer = WebRecord.mediaSource.addSourceBuffer('video/webm; codecs="vp8"');
		  console.log('Source buffer: ', WebRecord.sourceBuffer);
		}

		function handleDataAvailable(event) {
		  if (event.data && event.data.size > 0) {
			WebRecord.recordedBlobs.push(event.data);
		  }
		}

		function handleStop(event) {
		  console.log('Recorder stopped: ', event);
		  const superBuffer = new Blob(WebRecord.recordedBlobs, {type: 'video/webm'});
		  WebRecord.video.src = window.URL.createObjectURL(superBuffer);
		}

		function toggleRecording() {
		  if (WebRecord.recordButton.textContent === 'Start Recording') {
			startRecording();
		  } else {
			stopRecording();
			WebRecord.recordButton.textContent = 'Start Recording';
			WebRecord.playButton.disabled = false;
			WebRecord.downloadButton.disabled = false;
		  }
		}

		// The nested try blocks will be simplified when Chrome 47 moves to Stable
		function startRecording() {
		  var options = {mimeType: 'video/webm'};
		  WebRecord.recordedBlobs = [];
		  try {
			WebRecord.mediaRecorder = new MediaRecorder(WebRecord.mediaStream, options);
		  } catch (e0) {
			console.log('Unable to create MediaRecorder with options Object: ', e0);
			try {
			  options = {mimeType: 'video/webm,codecs=vp9'};
			  WebRecord.mediaRecorder = new MediaRecorder(WebRecord.mediaStream, options);
			} catch (e1) {
			  console.log('Unable to create MediaRecorder with options Object: ', e1);
			  try {
				options = 'video/vp8'; // Chrome 47
				WebRecord.mediaRecorder = new MediaRecorder(WebRecord.mediaStream, options);
			  } catch (e2) {
				alert('MediaRecorder is not supported by this browser.\n\n' +
				  'Try Firefox 29 or later, or Chrome 47 or later, ' +
				  'with Enable experimental Web Platform features enabled from chrome://flags.');
				console.error('Exception while creating MediaRecorder:', e2);
				return;
			  }
			}
		  }
		  console.log('Created MediaRecorder', WebRecord.mediaRecorder, 'with options', options);
		  WebRecord.recordButton.textContent = 'Stop Recording';
		  WebRecord.playButton.disabled = true;
		  WebRecord.downloadButton.disabled = true;
		  WebRecord.mediaRecorder.onstop = handleStop;
		  WebRecord.mediaRecorder.ondataavailable = handleDataAvailable;
		  WebRecord.mediaRecorder.start(100); // collect 100ms of data
		  console.log('MediaRecorder started', WebRecord.mediaRecorder);
		}

		function stopRecording() {
		  WebRecord.mediaRecorder.stop();
		  console.log('Recorded Blobs: ', WebRecord.recordedBlobs);
		  WebRecord.video.controls = true;
		}

		function play() {
		  WebRecord.video.play();
		}

		function download() {
		  var blob = new Blob(WebRecord.recordedBlobs, {type: 'video/webm'});
		  var url = window.URL.createObjectURL(blob);
		  var a = document.createElement('a');
		  a.style.display = 'none';
		  a.href = url;
		  a.download = 'test.webm';
		  document.body.appendChild(a);
		  a.click();
		  
		  setTimeout(removeInpl, 100);
		}

		function removeImpl()
		{
			document.body.removeChild(a);
			window.URL.revokeObjectURL(url);
		}

		WebRecord.mediaSource = new MediaSource();
		WebRecord.mediaSource.addEventListener('sourceopen', handleSourceOpen, false);

		WebRecord.canvas = document.querySelector('canvas');
		WebRecord.video = document.querySelector('video');

		WebRecord.recordButton = document.querySelector('button#record');
		WebRecord.playButton = document.querySelector('button#play');
		WebRecord.downloadButton = document.querySelector('button#download');
		
		WebRecord.recordButton.onclick = toggleRecording;
		WebRecord.playButton.onclick = play;
		WebRecord.downloadButton.onclick = download;

		WebRecord.canvasStream = WebRecord.canvas.captureStream(); // frames per second
		console.log('Started stream capture from canvas element: ', WebRecord.canvasStream);

		// for Audio

		WebRecord.audioContext = WEBAudio.audioContext;
		WebRecord.audioNode = WEBAudio.audioInstances[1].gain;

		WebRecord.destination = WebRecord.audioContext.createMediaStreamDestination();
		
		WebRecord.audioNode.connect(WebRecord.destination);
		WebRecord.oscillator = WebRecord.audioContext.createOscillator();
		WebRecord.oscillator.connect(WebRecord.destination);

		WebRecord.audioStream = WebRecord.destination.stream;

		WebRecord.mediaStream = new MediaStream();

		var canvasTracks = WebRecord.canvasStream.getTracks();
		for(var i = 0; i < canvasTracks.length; i++)
		{
			WebRecord.mediaStream.addTrack(canvasTracks[i]);
		}

		var audioTracks = WebRecord.audioStream.getTracks();
		for(var i = 0; i < audioTracks.length; i++)
		{
			WebRecord.mediaStream.addTrack(audioTracks[i]);
		}
	},
};

autoAddDeps(WebRecordPlugin, '$WebRecord');
mergeInto(LibraryManager.library, WebRecordPlugin);