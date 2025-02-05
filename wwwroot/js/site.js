var authToken = document.cookie.split('; ').find(row => row.startsWith('AuthToken='));

if (authToken) {
    var tokenValue = authToken.split('=')[1];

    try {
        var decodedToken = JSON.parse(atob(tokenValue.split('.')[1]));
        var expireTimestamp = decodedToken.exp * 1000;

        function updateRemainingTime() {
            var remainingTime = expireTimestamp - new Date().getTime();
            console.log(remainingTime);
            if (remainingTime > 0) {
                var minutes = Math.floor(remainingTime / 60000);
                var seconds = Math.floor((remainingTime % 60000) / 1000);
                var expireElement = document.querySelector('.expireTime');
                if (expireElement) {
                    expireElement.textContent = `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
                }
            } else {
                console.log('Token süresi doldu. Sayfa yenileniyor...');
                window.location.href = '/User/Login';
            }

        }

        updateRemainingTime();
        setInterval(updateRemainingTime, 1000);

    } catch (error) {
        console.log("Geçersiz token:", error);
    }
} else {
    console.log('AuthToken çerezi bulunamadı!');
}
const form = document.querySelector(".toast-header");
if (form) {
    var toast = new bootstrap.Toast(document.querySelector('.toast'));
    toast.show();
}

const book = document.querySelector(".book-container");
const author = document.querySelector(".author-container");

if (book) {
    document.querySelectorAll('.card-img-top').forEach((img, index) => {
        img.src = `https://picsum.photos/id/${index + 5}/200/300`;
    });
} else if (author) {
    document.querySelectorAll('.card-img-top').forEach((img, index) => {
        img.src = `https://picsum.photos/id/${index + 7}/200/300`;
    });
}

document.getElementById("deleteButton").addEventListener("click", function (event) {
    const bookId = event.target.getAttribute("data-book-id");

    
    const modal = new bootstrap.Modal(document.getElementById("confirmDeleteModal"));
    modal.show();

   
    document.getElementById("confirmDeleteButton").addEventListener("click", function () {
        
        window.location.href = '/Book/Remove/' + bookId;
    });
});



