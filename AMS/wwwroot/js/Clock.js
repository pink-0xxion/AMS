$(document).ready(() => {


    //Live clock 
    function updateClock() {
        const now = new Date();

        const options = {
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            hour12: true,
            weekday: 'long',
            day: '2-digit',
            month: 'long',
            year: 'numeric',

            timeZone: 'Asia/Kolkata' // Indian Standard Time
        };

        const formattedTime = now.toLocaleString('en-IN', options);
        document.getElementById('liveClock').textContent = formattedTime;
    }

    setInterval(updateClock, 1000); // update every second
    updateClock(); // initial call



});
