document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('orderForm');
    const modal = document.getElementById('modal_confirm');
    const closeBtn = document.getElementsByClassName('modal_confirm-close-btn')[0];
    const confirmBtn = document.getElementById('confirmBtn');
    const cancelBtn = document.getElementById('cancelBtn');
    const cancelOrderLink = document.getElementById('cancelOrderLink');
    let href;
    if (form != null) {
        form.onsubmit = function (event) {
            event.preventDefault();
            modal.style.display = 'block';
        }

        confirmBtn.onclick = function () {
            form.submit();
            modal.style.display = 'none';
        }

    }
    
    if (cancelOrderLink != null) {
        cancelOrderLink.onclick = function (event) {
            event.preventDefault();
            href = cancelOrderLink.getAttribute('href');
            modal.style.display = 'block';
        }

        confirmBtn.onclick = function () {
            window.location.href = href;
            modal.style.display = 'none';
        }
    }
    closeBtn.onclick = function () {
        modal.style.display = 'none';
    }

   
    cancelBtn.onclick = function () {
        modal.style.display = 'none';
    }

    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    }
});