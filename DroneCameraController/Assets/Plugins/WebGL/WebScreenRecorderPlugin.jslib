var WebRecordPlugin =
{
	$WebRecord: {
		mediaRecorder: {},
		recordedBlobs: {},
		sourceBuffer: {},
		video: {},
		canvas: {},
		mediaSource: {},
		// recordButton: {},
		// playButton: {},
		// downloadButton: {},
		canvasStream: {},
		audioContext: {},
		audioNode: {},
		destination: {},
		oscillator: {},
		audioStream: {},
		mediaStream: {},
		recordingState: "Start Recording",
	},

	// Unity側から呼ばれる
	StartOrStopRecording: function () {

		toggleRecording();

		// 録画のStart/Stop
		function toggleRecording() {
			if (WebRecord.recordingState == 'Start Recording') {
				initializeRecorder();
				startRecording();
			} else {
				stopRecording();
				download();
				WebRecord.recordingState = 'Start Recording';
				// WebRecord.playButton.disabled = false;
				// WebRecord.downloadButton.disabled = false;
			}
		}

		// 初期化処理(録画開始前に必ず呼ぶ)
		function initializeRecorder() {
			WebRecord.mediaSource = new MediaSource();
			WebRecord.mediaSource.addEventListener('sourceopen', handleSourceOpen, false);

			WebRecord.canvas = document.querySelector('canvas');
			// WebRecord.video = document.querySelector('video');

			// WebRecord.recordButton = document.querySelector('button#record');
			// WebRecord.playButton = document.querySelector('button#play');
			// WebRecord.downloadButton = document.querySelector('button#download');

			// WebRecord.recordButton.onclick = toggleRecording;
			// WebRecord.playButton.onclick = play;
			// WebRecord.downloadButton.onclick = download;

			WebRecord.canvasStream = WebRecord.canvas.captureStream(); // frames per second
			console.log('Started stream capture from canvas element: ', WebRecord.canvasStream);

			// Record Audio
			WebRecord.audioContext = WEBAudio.audioContext;
			WebRecord.destination = WebRecord.audioContext.createMediaStreamDestination();

			// 録音の流れ
			// audioのinstanceを取得し、それらのgainノードを録画用のdestinatationに接続することで
			// Audioの録音が可能に
			// WEBAudio.audioInstances : Unity内(STYLY Plugin含む)であらかじめ設定されたAudio Source経由のもの、サンクラ経由のもの
			// document.querySelectAll('audio') : STYLY Editor上でmp3アップロードされたもの

			// for Unity Audio
			for (var i = 0; i < WEBAudio.audioInstances.length; i++) {
				if (WEBAudio.audioInstances[i].gain != null) {
					WEBAudio.audioInstances[i].gain.connect(WebRecord.destination);
				}
			}

			// for STYLY Audio
			// var audioList = document.querySelectorAll('audio');

			// for (var i = 0; i < audioInstances.length; i++) {
			//	if (audioInstances[i].gain != null) {
			//		audioInstances[i].gain.connect(WebRecord.destination);
			//	}
			// }

			WebRecord.oscillator = WebRecord.audioContext.createOscillator();
			WebRecord.oscillator.connect(WebRecord.destination);
			WebRecord.audioStream = WebRecord.destination.stream;
			WebRecord.mediaStream = new MediaStream();

			var canvasTracks = WebRecord.canvasStream.getTracks();
			for (var i = 0; i < canvasTracks.length; i++) {
				WebRecord.mediaStream.addTrack(canvasTracks[i]);
			}

			var audioTracks = WebRecord.audioStream.getTracks();
			for (var i = 0; i < audioTracks.length; i++) {
				WebRecord.mediaStream.addTrack(audioTracks[i]);
			}
		}

		// データが開かれた時のイベントハンドラ
		function handleSourceOpen(event) {
			console.log('MediaSource opened');
			WebRecord.sourceBuffer = WebRecord.mediaSource.addSourceBuffer('video/webm; codecs="vp8"');
			console.log('Source buffer: ', WebRecord.sourceBuffer);
		}

		// データが利用可能になった時のイベントハンドラ
		function handleDataAvailable(event) {
			if (event.data && event.data.size > 0) {
				WebRecord.recordedBlobs.push(event.data);
			}
		}

		// 録画停止時のイベントハンドラ
		function handleStop(event) {
			console.log('Recorder stopped: ', event);
			const superBuffer = new Blob(WebRecord.recordedBlobs, { type: 'video/webm' });
			// WebRecord.video.src = window.URL.createObjectURL(superBuffer);
		}

		// The nested try blocks will be simplified when Chrome 47 moves to Stable
		function startRecording() {
			var options = { mimeType: 'video/webm' };
			WebRecord.recordedBlobs = [];
			try {
				WebRecord.mediaRecorder = new MediaRecorder(WebRecord.mediaStream, options);
			} catch (e0) {
				console.log('Unable to create MediaRecorder with options Object: ', e0);
				try {
					options = { mimeType: 'video/webm,codecs=vp9' };
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
			WebRecord.recordingState = 'Stop Recording';
			// WebRecord.playButton.disabled = true;
			// WebRecord.downloadButton.disabled = true;
			WebRecord.mediaRecorder.onstop = handleStop;
			WebRecord.mediaRecorder.ondataavailable = handleDataAvailable;
			WebRecord.mediaRecorder.start(100); // collect 100ms of data
			console.log('MediaRecorder started', WebRecord.mediaRecorder);
		}

		function stopRecording() {
			WebRecord.mediaRecorder.stop();
			console.log('Recorded Blobs: ', WebRecord.recordedBlobs);
			// WebRecord.video.controls = true;
		}

		function play() {
			WebRecord.video.play();
		}

		function download() {
			var blob = new Blob(WebRecord.recordedBlobs, { type: 'video/webm' });
			var url = window.URL.createObjectURL(blob);
			var a = document.createElement('a');
			a.style.display = 'none';
			a.href = url;
			var fileName = getTodayDate() + ".webm";
			a.download = fileName;
			document.body.appendChild(a);
			a.click();

			setTimeout(function () {
				document.body.removeChild(a);
				window.URL.revokeObjectURL(url);
			}, 100);
		}

		function getTodayDate() {
			var today = new Date();
			var year = today.getFullYear().toString();
			var month = (today.getMonth() + 1).toString();
			var day = today.getDate().toString();
			var hour = today.getHours().toString();
			var minute = today.getMinutes().toString();
			var second = today.getSeconds();

			var todayDate = year + "_" + month + "_" + day + "_" + hour + "_" + minute + "_" + second;

			return todayDate;
		}
	},
};

autoAddDeps(WebRecordPlugin, '$WebRecord');
mergeInto(LibraryManager.library, WebRecordPlugin);