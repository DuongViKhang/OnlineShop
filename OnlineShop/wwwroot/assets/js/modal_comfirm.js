document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('orderForm');
    const modal = document.getElementById('modal_confirm');
    const closeBtn = document.getElementsByClassName('modal_confirm-close-btn')[0];
    const confirmBtn = document.getElementById('confirmBtn');
    const cancelBtn = document.getElementById('cancelBtn');
    const confirm_link = document.getElementById('confirm_link');
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
    
    if (confirm_link != null) {
        confirm_link.onclick = function (event) {
            event.preventDefault();
            href = cancelOrderLink.getAttribute('href');
            var dataId = confirm_link.getAttribute('data-id');
            document.getElementById('modal_text').textContent = "Bạn chắc chắn nhận giao đơn hàng #"+dataId+ " chứ?"
            modal.style.display = 'block';
        }

        confirmBtn.onclick = function () {
            window.location.href = href;
            modal.style.display = 'none';
        }
    }
    if (form != null || confirm_link != null) {
        closeBtn.onclick = function () {
            modal.style.display = 'none';
        }


        cancelBtn.onclick = function () {
            modal.style.display = 'none';
        }

    }
    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    }
});