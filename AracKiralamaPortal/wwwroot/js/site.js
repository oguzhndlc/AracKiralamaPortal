// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Toast container
function showToast(message) {
    let container = document.querySelector(".toast-container");
    if (!container) {
        container = document.createElement("div");
        container.className = "toast-container";
        document.body.appendChild(container);
    }

    const toast = document.createElement("div");
    toast.className = "toast-notification";
    toast.innerText = message;

    // Yeni toast en alta eklensin
    container.appendChild(toast);

    // Mevcut toast'ları yukarı taşı
    Array.from(container.children).forEach((t, i) => {
        // Öncekilerden biraz daha az yukarı kaydır
        t.style.transform = `translateY(-${i}px)`; // Her toast arası 50px
    });


    setTimeout(() => toast.classList.add("show"), 10);

    setTimeout(() => {
        toast.classList.remove("show");
        setTimeout(() => toast.remove(), 400);
    }, 3000);
}

// ModelState hatalarını veya success mesajlarını göstermek için
function showTempDataToasts(errors = [], success = "") {
    if (Array.isArray(errors) && errors.length > 0) {
        errors.forEach(e => showToast(e));
    }
    if (success) {
        showToast(success);
    }
}


function ajaxLogout() {
    fetch('/Account/LogoutAjax', {
        method: 'POST',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                showToast("Çıkış yapıldı 👋");

                setTimeout(() => {
                    window.location.href = "/";
                }, 1200);
            }
        })
        .catch(() => {
            showToast("Çıkış yapılırken hata oluştu ❌");
        });
}

