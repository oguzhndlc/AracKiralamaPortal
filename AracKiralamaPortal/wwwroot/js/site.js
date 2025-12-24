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

    container.appendChild(toast);

    Array.from(container.children).forEach((t, i) => {
        t.style.transform = `translateY(-${i}px)`;
    });


    setTimeout(() => toast.classList.add("show"), 10);

    setTimeout(() => {
        toast.classList.remove("show");
        setTimeout(() => toast.remove(), 400);
    }, 3000);
}

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

