
// =============================================
// Tab Visibility – Auto Pause/Resume Audio/Video
// =============================================
document.addEventListener('DOMContentLoaded', function() {
    let mediaPlayingState = [];

    function pauseAllMedia() {
        const mediaElements = document.querySelectorAll('audio, video');
        mediaPlayingState = [];
        mediaElements.forEach(media => {
            const wasPlaying = !media.paused;
            mediaPlayingState.push({ element: media, wasPlaying: wasPlaying });
            if (wasPlaying) {
                media.pause();
            }
        });
    }

    function resumeAllMedia() {
        mediaPlayingState.forEach(item => {
            if (item.wasPlaying) {
                item.element.play().catch(e => console.warn('Could not resume media:', e));
            }
        });
        mediaPlayingState = [];
    }

    document.addEventListener('visibilitychange', function() {
        if (document.hidden) {
            pauseAllMedia();
        } else {
            resumeAllMedia();
        }
    });
});

document.addEventListener('keydown', function (e) {
    const tag = e.target.tagName.toLowerCase();
    // Ignore if focused on input, textarea, or contenteditable
    if (tag === 'input' || tag === 'textarea' || e.target.isContentEditable) {
        return;
    }
    if ((e.ctrlKey || e.metaKey) && e.key === 'p') {
        e.preventDefault();
        window.location.href = '/Home/Product';
    }
});
