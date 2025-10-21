/**
 * Puantaj Common Functions
 * Shared functionality for both Create and Index pages
 */

// Global variables that can be shared
let currentYear;
let currentMonth;
let expenseData = {};
let currentEditingDay = null;

/**
 * Utility Functions
 */
function getDayOfWeekTurkish(day) {
    const days = ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'];
    return days[day];
}

function formatTimeSpan(timeSpan) {
    if (!timeSpan || timeSpan == null) return '';

    let formattedTime = '';

    // If timeSpan is already in HH:mm format, return as is
    if (typeof timeSpan === 'string' && timeSpan.includes(':')) {
        formattedTime = timeSpan.substring(0, 5); // Get HH:mm part
    }
    // If timeSpan is in .NET TimeSpan format (e.g., "08:00:00")
    else if (typeof timeSpan === 'string') {
        const parts = timeSpan.split(':');
        if (parts.length >= 2) {
            formattedTime = `${parts[0].padStart(2, '0')}:${parts[1].padStart(2, '0')}`;
        }
    }

    // Convert 00:00 to 23:59 since 00:00 is not a valid time in the system
    if (formattedTime === '00:00') {
        formattedTime = '23:59';
    }

    return formattedTime;
}

/**
 * Converts 00:00 time to 23:59 for display purposes
 * @param {string} timeString - Time string in HH:mm format
 * @returns {string} - Converted time string
 */
function convertMidnightTime(timeString) {
    if (!timeString || typeof timeString !== 'string') return timeString;
    
    // Check if the time is exactly 00:00
    if (timeString.trim() === '00:00') {
        return '23:59';
    }
    
    return timeString;
}

/**
 * Validates and converts time input to prevent 00:00 entry
 * @param {string} timeValue - Time value to validate
 * @returns {string} - Validated and converted time value
 */
function validateAndConvertTime(timeValue) {
    if (!timeValue || typeof timeValue !== 'string') return timeValue;
    
    // Remove any whitespace
    timeValue = timeValue.trim();
    
    // If user enters 00:00, convert to 23:59
    if (timeValue === '00:00') {
        return '23:59';
    }
    
    return timeValue;
}

function setupAdditionalTimeHandlers() {
    // Ek saat buton event listener
    $(document).on('click', '.additional-time-btn', function () {
        const day = $(this).data('day');
        const additionalSection = $(`.additional-time-section[data-day="${day}"]`);

        if (additionalSection.is(':visible')) {
            additionalSection.hide();
            $(this).removeClass('btn-success').addClass('btn-outline-success');
            $(this).find('i').removeClass('fa-minus').addClass('fa-plus');
        } else {
            additionalSection.show();
            $(this).removeClass('btn-outline-success').addClass('btn-success');
            $(this).find('i').removeClass('fa-plus').addClass('fa-minus');
        }
    });

    // Saat input'ları için focus event handler
    $(document).on('focus', '.time-input, .additional-start-time, .additional-end-time', function () {
        const input = $(this)[0];
        const value = input.value;

        // Eğer sadece iki nokta varsa, iki noktayı da sil ve baştan yazıyormuş gibi yap
        if (value === ':') {
            input.value = '';
            input.setSelectionRange(0, 0);
        }
    });

    // Saat input'ları için keyup event handler (silme işleminden sonra)
    $(document).on('keyup', '.time-input, .additional-start-time, .additional-end-time', function (e) {
        const input = $(this)[0];
        const value = input.value;

        // Backspace veya Delete tuşuna basıldıktan sonra sadece iki nokta kaldıysa, iki noktayı da sil
        if ((e.keyCode === 8 || e.keyCode === 46) && value === ':') {
            input.value = '';
            input.setSelectionRange(0, 0);
        }
    });

    // Saat input'ları için click event handler
    $(document).on('click', '.time-input, .additional-start-time, .additional-end-time', function () {
        const input = $(this)[0];
        const value = input.value;

        // Eğer sadece iki nokta varsa, iki noktayı da sil ve baştan yazıyormuş gibi yap
        if (value === ':') {
            input.value = '';
            input.setSelectionRange(0, 0);
        }
    });

    // Saat input'ları için özel event handler'lar
    $(document).on('input', '.time-input, .additional-start-time, .additional-end-time', function () {
        let value = $(this).val();
        const input = $(this)[0];
        const cursorPos = input.selectionStart;

        // Sadece sayı ve iki nokta kalsın
        value = value.replace(/[^0-9:]/g, '');

        // Nokta, virgül veya boşluk tuşuna basıldığında iki nokta koy
        if (value.includes('.') || value.includes(',') || value.includes(' ')) {
            value = value.replace(/[.,\s]/g, ':');
        }

        // Maksimum 5 karakter (HH:MM)
        if (value.length > 5) {
            value = value.substring(0, 5);
        }

        // Otomatik iki nokta ekleme (2 karakterden sonra)
        if (value.length === 2 && !value.includes(':')) {
            value += ':';
        }

        // 00:00 girişini engelle ve 23:59'a çevir
        value = validateAndConvertTime(value);

        $(this).val(value);

        // Eğer sadece iki nokta varsa, iki noktayı da sil ve baştan yazıyormuş gibi yap
        if (value === ':') {
            input.value = '';
            input.setSelectionRange(0, 0);
        }
    });

    // Saat input'ları için gelişmiş keydown handler
    $(document).on('keydown', '.time-input, .additional-start-time, .additional-end-time', function (e) {
        const input = $(this);
        const value = input.val();
        const cursorPos = input[0].selectionStart;

        // Backspace, Delete, Arrow keys, Tab, Enter tuşlarına izin ver
        if ([8, 9, 13, 27, 37, 38, 39, 40].indexOf(e.keyCode) !== -1) {
            // Backspace tuşu için özel işlem
            if (e.keyCode === 8) { // Backspace
                e.preventDefault();

                if (cursorPos > 0) {
                    let newValue = value;

                    // Eğer iki noktadan sonraki karakter siliniyorsa
                    if (cursorPos > value.indexOf(':') && value.indexOf(':') !== -1) {
                        // İki noktadan sonraki tüm karakterleri sil
                        const colonPos = value.indexOf(':');
                        newValue = value.substring(0, colonPos + 1);
                    } else if (cursorPos === value.indexOf(':') + 1 && value.indexOf(':') !== -1) {
                        // İki noktayı sil
                        newValue = value.substring(0, value.indexOf(':'));
                    } else {
                        // Normal silme
                        newValue = value.substring(0, cursorPos - 1) + value.substring(cursorPos);
                    }

                    input.val(newValue);
                    input[0].setSelectionRange(cursorPos - 1, cursorPos - 1);
                }
                return;
            }

            // Delete tuşu için özel işlem
            if (e.keyCode === 46) { // Delete
                e.preventDefault();

                if (cursorPos < value.length) {
                    let newValue = value;

                    // Eğer iki noktadan önceki karakter siliniyorsa
                    if (cursorPos < value.indexOf(':') && value.indexOf(':') !== -1) {
                        // İki noktadan önceki tüm karakterleri sil
                        const colonPos = value.indexOf(':');
                        newValue = value.substring(colonPos);
                    } else {
                        // Normal silme
                        newValue = value.substring(0, cursorPos) + value.substring(cursorPos + 1);
                    }

                    input.val(newValue);
                    input[0].setSelectionRange(cursorPos, cursorPos);
                }
                return;
            }

            return;
        }

        // Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X tuşlarına izin ver
        if ((e.ctrlKey || e.metaKey) && [65, 67, 86, 88].indexOf(e.keyCode) !== -1) {
            return;
        }

        // Sadece sayı tuşlarına izin ver (hem sol hem sağ taraftaki)
        if (e.keyCode >= 48 && e.keyCode <= 57) { // 0-9 (sol taraf)
            // Sayı girişi kontrolü
            const colonPos = value.indexOf(':');

            if (colonPos === -1) {
                // Henüz iki nokta yok, maksimum 2 karakter
                if (value.length >= 2) {
                    e.preventDefault();
                }
            } else {
                // İki nokta var, pozisyona göre kontrol
                if (cursorPos <= colonPos) {
                    // İki noktadan önce, maksimum 2 karakter
                    if (value.substring(0, colonPos).length >= 2) {
                        e.preventDefault();
                    }
                } else {
                    // İki noktadan sonra, maksimum 2 karakter
                    if (value.substring(colonPos + 1).length >= 2) {
                        e.preventDefault();
                    }
                }
            }
        } else if (e.keyCode >= 96 && e.keyCode <= 105) { // 0-9 (NumLock)
            // NumLock açıkken sağ taraftaki sayı tuşları
            const colonPos = value.indexOf(':');

            if (colonPos === -1) {
                // Henüz iki nokta yok, maksimum 2 karakter
                if (value.length >= 2) {
                    e.preventDefault();
                }
            } else {
                // İki nokta var, pozisyona göre kontrol
                if (cursorPos <= colonPos) {
                    // İki noktadan önce, maksimum 2 karakter
                    if (value.substring(0, colonPos).length >= 2) {
                        e.preventDefault();
                    }
                } else {
                    // İki noktadan sonra, maksimum 2 karakter
                    if (value.substring(colonPos + 1).length >= 2) {
                        e.preventDefault();
                    }
                }
            }
        } else if (e.keyCode === 190 || e.keyCode === 188 || e.keyCode === 32) { // Nokta, virgül, boşluk
            // İki nokta ekleme
            if (value.indexOf(':') === -1 && value.length === 2) {
                // İki nokta ekle
                e.preventDefault();
                let newValue = value + ':';
                // 00:00 girişini engelle
                if (newValue === '00:') {
                    newValue = '23:';
                }
                input.val(newValue);
                input[0].setSelectionRange(3, 3);
            } else {
                e.preventDefault();
            }
        } else {
            // Diğer tuşları engelle
            e.preventDefault();
        }
    });

    // Saat input'ları için blur event handler (00:00 girişini yakalamak için)
    $(document).on('blur', '.time-input, .additional-start-time, .additional-end-time', function () {
        let value = $(this).val();
        if (value === '00:00') {
            $(this).val('23:59');
        }
    });
}

function clearExpenseForm() {
    $('#expenseForm')[0].reset();
    $('#expenseForm').removeClass('was-validated');
    $('#expense-select').val('').trigger('change');
}

function showTableLoading() {
    $('#dynamicTableContainer').addClass('table-loading');
    $('#tableLoadingSpinner').show();
    $('#tableContent').hide();
}

function hideTableLoading() {
    $('#dynamicTableContainer').removeClass('table-loading');
    $('#tableLoadingSpinner').hide();
    $('#tableContent').show();
}

/**
 * Select2 Management Functions
 */
async function initializeEquipmentSelect() {
    try {
        const response = await $.ajax({
            url: '/ekipman/aktif-liste',
            type: 'GET',
            dataType: 'json'
        });

        if (response && response.isSuccess && response.data) {
            $('.equipment-select').each(function () {
                const currentSelect = $(this);
                const currentValue = currentSelect.data('current-value');

                currentSelect.empty();
                currentSelect.append('<option value="">Ekipman seçin</option>');
                response.data.forEach(equipment => {
                    const isSelected = currentValue === equipment.id;
                    currentSelect.append(`<option value="${equipment.id}" ${isSelected ? 'selected' : ''}>${equipment.name}</option>`);
                });
            });
        }

        $('.equipment-select').select2({
            placeholder: 'Ekipman seçin',
            theme: 'bootstrap-5',
            allowClear: true,
            width: '100%'
        });
    } catch (error) {
        console.error('Ekipman listesi yüklenirken hata:', error);
        toastr.error('Ekipman listesi yüklenirken hata oluştu', 'Hata!');
    }
}

function initializeLocationSelects() {
    $('.province-select').select2({
        placeholder: 'İl seçin',
        theme: 'bootstrap-5',
        allowClear: true,
        width: '100%'
    });

    $('.district-select').select2({
        placeholder: 'İlçe seçin',
        theme: 'bootstrap-5',
        allowClear: true,
        width: '100%'
    });
}

async function initializeExpenseSelect() {
    try {
        const response = await $.ajax({
            url: '/masraf/aktif-liste',
            type: 'GET',
            dataType: 'json'
        });

        if (response && response.isSuccess && response.data) {
            $('#expense-select').empty();
            $('#expense-select').append('<option value="">Masraf türü seçin</option>');

            response.data.forEach(expenseType => {
                $('#expense-select').append(`<option value="${expenseType.id}">${expenseType.name}</option>`);
            });
        }

        $('#expense-select').select2({
            placeholder: 'Masraf türü seçin',
            theme: 'bootstrap-5',
            dropdownParent: $('#expenseModal'),
            width: '100%'
        });
    } catch (error) {
        console.error('Masraf türleri yüklenirken hata:', error);
        toastr.error('Masraf türleri yüklenirken hata oluştu', 'Hata!');
    }
}

async function initializeProjectSelect() {
    try {
        const response = await $.ajax({
            url: '/proje/aktif-liste',
            type: 'GET',
            dataType: 'json'
        });

        if (response && response.isSuccess && response.data) {
            $('.project-select').each(function () {
                const currentSelect = $(this);
                const currentValue = currentSelect.data('current-value');

                currentSelect.empty();
                currentSelect.append('<option value="">Proje seçin</option>');
                response.data.forEach(project => {
                    const isSelected = currentValue === project.id;
                    currentSelect.append(`<option value="${project.id}" ${isSelected ? 'selected' : ''}>${project.name}</option>`);
                });
            });
        }

        $('.project-select').select2({
            placeholder: 'Proje seçin',
            theme: 'bootstrap-5',
            allowClear: true,
            width: '100%'
        });
    } catch (error) {
        console.error('Proje listesi yüklenirken hata:', error);
        toastr.error('Proje listesi yüklenirken hata oluştu', 'Hata!');
    }
}
async function initializeExcuseReasonSelect() {
    try {
        const reasons = [
            { value: 'Hafta Tatili', text: 'Hafta Tatili' },
            { value: 'Rapor', text: 'Rapor' },
            { value: 'Ücretsiz İzin', text: 'Ücretsiz İzin' },
            { value: 'Yıllık İzin', text: 'Yıllık İzin' },
            { value: 'Babalık İzni', text: 'Babalık İzni' },
            { value: 'İdari İzin', text: 'İdari İzin (Serbest Zaman İzni)' },
            { value: 'Cenaze İzni', text: 'Cenaze İzni' },
            { value: 'Evlilik İzni', text: 'Evlilik İzni' },
            { value: 'Süt İzni', text: 'Süt İzni' }
        ];
        if (!window.isUserPersonel) {
            reasons.push({ value: 'Devamsızlık', text: 'Devamsızlık' });
        }
        $('.excuse-reason-select').each(function () {
            const currentSelect = $(this);
            const currentValue = currentSelect.data('current-value');

            currentSelect.empty();
            currentSelect.append('<option value=""></option>');

            reasons.forEach(reason => {
                const isSelected = currentValue === reason.value;
                currentSelect.append(`<option value="${reason.value}" ${isSelected ? 'selected' : ''}>${reason.text}</option>`);
            });
        });

        $('.excuse-reason-select').select2({
            placeholder: 'Sebep seçin',
            theme: 'bootstrap-5',
            allowClear: true,
            width: '100%'
        });
    } catch (error) {
        console.error('İzin sebep listesi yüklenirken hata:', error);
        toastr.error('İzin sebep listesi yüklenirken hata oluştu', 'Hata!');
    }
}

function initializeUserSelect() {
    $('.user-select').select2({
        placeholder: 'Kullanıcı seçin',
        theme: 'bootstrap-5',
        width: '100%'
    });
}


async function initializeAllSelects() {
    await initializeEquipmentSelect();
    initializeLocationSelects();
    await initializeExpenseSelect();
    await initializeProjectSelect();
    await initializeExcuseReasonSelect();
}

/**
 * Expense Management Functions
 */
function setupExpenseModal() {
    $('#expenseModal').on('show.bs.modal', function (event) {
        const button = $(event.relatedTarget);
        currentEditingDay = button.data('day');

        if (currentEditingDay) {
            const date = new Date(currentYear, currentMonth - 1, currentEditingDay);
            const dateStr = `${currentEditingDay.toString().padStart(2, '0')}.${currentMonth.toString().padStart(2, '0')}.${currentYear} ${getDayOfWeekTurkish(date.getDay())}`;
            $('#modalDateInfo').text(dateStr);

            loadExpensesForDay(currentEditingDay);
            clearExpenseForm();
        }
    });
}

function setupExpenseEventListeners() {
    // Form submit işlemi
    $('#expenseForm').on('submit', function (e) {
        e.preventDefault();

        if (this.checkValidity()) {
            addExpenseToDay();
        }

        $(this).addClass('was-validated');
    });

    // Masraf silme butonları
    $(document).on('click', '.remove-expense-btn', function () {
        const expenseIndex = $(this).data('index');
        removeExpenseFromDay(expenseIndex);
    });

  
}


function loadExpensesForDay(day) {
    if (!expenseData[day]) {
        expenseData[day] = [];
    }

    displayCurrentExpenses(day);
    updateTotalExpense(day);
}
function displayCurrentExpenses(day, isApproved = false) {
    const expenses = expenseData[day] || [];
    const expensesList = $('#expenseItemsList');
    const currentExpensesSection = $('#currentExpensesList');
    expensesList.empty();

    if (expenses.length > 0) {
        currentExpensesSection.show();
        expenses.forEach((expense, index) => {
            const expenseName = expense.expenseName
                ?? $('#expense-select option[value="' + expense.expenseId + '"]').text();

            const actionButton = isApproved
                ? `<span class="badge bg-success">
                        <i class="fas fa-check me-1"></i>Onaylandı
                    </span>`
                : `<button type="button" class="btn btn-sm btn-outline-danger remove-expense-btn"
                            data-index="${index}" title="Masrafı Sil">
                        <i class="fas fa-trash"></i>
                    </button>`;

            // Dosya gösterimi
            let fileDisplay = '';
            if (expense.fileId) {
                fileDisplay = `
                    <br/>
                    <a href="/dosya/indir/${expense.fileId}"
                       download
                       class="text-muted text-decoration-underline">
                        <i class="fas fa-download me-1"></i>
                        ${expense.originalFileName}
                    </a>
                `;
            } else if (expense.receiptFile && expense.receiptFile.name) {
                fileDisplay = `
                    <br/>
                    <small class="text-muted">${expense.receiptFile.name}</small>
                `;
            }

            const expenseItem = `
                <div class="list-group-item d-flex justify-content-between align-items-center">
                    <div>
                        <h6 class="mb-1">${expenseName}</h6>
                        <p class="mb-1"><strong>${expense.amount.toFixed(2)} ₺</strong></p>
                        ${expense.description ? `<small class="text-muted">${expense.description}</small>` : ''}
                        ${fileDisplay}
                    </div>
                    ${actionButton}
                </div>
            `;
            expensesList.append(expenseItem);
        });
    } else {
        currentExpensesSection.hide();
    }
}
function downloadFile(fileId, fileName) {
    $.ajax({
        url: `/dosya/indir/${fileId}`,
        method: 'GET',
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            const url = window.URL.createObjectURL(data);
            const a = document.createElement('a');
            a.href = url;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();
        },
        error: function () {
            toastr.error('Dosya indirilemedi');
        }
    });
}
function addExpenseToDay() {
    const expenseId = $('#expense-select').val();
    const amount = parseFloat($('#txtAmount').val());
    const description = $('#txtDescription').val();
    const receiptFile = $('#fileReceipt')[0].files[0] ?? null;

    if (expenseId == null || expenseId == undefined || isNaN(amount) || amount == null) {
        toastr.warning("Masraf türünü ya da tutarı kontrol ediniz");
        return;
    }

    if (!receiptFile) {
        toastr.error("Masraf eklemek için belge yüklenmesi zorunludur!");
        return;
    }

    if (!expenseData[currentEditingDay]) {
        expenseData[currentEditingDay] = [];
    }

    const newExpense = {
        expenseId: expenseId,
        amount: amount,
        description: description,
        receiptFile: receiptFile
    };

    expenseData[currentEditingDay].push(newExpense);

    // Listeyi güncelle
    displayCurrentExpenses(currentEditingDay);
    updateTotalExpense(currentEditingDay);
    updateExpenseButton(currentEditingDay);

    // Formu temizle
    clearExpenseForm();

    toastr.success('Masraf eklendi!', 'Başarılı!');
}

function removeExpenseFromDay(index) {
    if (expenseData[currentEditingDay] && expenseData[currentEditingDay][index]) {
        expenseData[currentEditingDay].splice(index, 1);

        displayCurrentExpenses(currentEditingDay);
        updateTotalExpense(currentEditingDay);
        updateExpenseButton(currentEditingDay);

        toastr.info('Masraf silindi!', 'Bilgi');
    }
}

function updateTotalExpense(day) {
    const expenses = expenseData[day] || [];
    const total = expenses.reduce((sum, expense) => sum + expense.amount, 0);

    $('#totalExpenseAmount').text(total.toFixed(2) + ' ₺');

    if (total > 0) {
        $('#totalExpenseSection').show();
    } else {
        $('#totalExpenseSection').hide();
    }
}

function updateExpenseButton(day) {
    const expenses = expenseData[day] || [];
    const button = $(`.expense-modal-btn[data-day="${day}"]`);
    const count = expenses.length;
    const total = expenses.reduce((sum, expense) => sum + expense.amount, 0);

    // Show count and total amount with Turkish localization
    if (count > 0) {
        const formattedTotal = new Intl.NumberFormat('tr-TR', {
            style: 'currency',
            currency: 'TRY',
            minimumFractionDigits: 2
        }).format(total);
        
        button.find('.expense-count').html(`${count} Masraf<br/>${formattedTotal}`);
    } else {
        button.find('.expense-count').html('0 Masraf<br/>₺0,00');
    }

    if (count > 0) {
        button.removeClass('btn-outline-primary').addClass('btn-success');
    } else {
        button.removeClass('btn-success').addClass('btn-outline-primary');
    }
}

function setupExpenseManagement() {
    setupExpenseModal();
    setupExpenseEventListeners();
}

/**
 * Location Management Functions (extends turkey-locations.js)
 */
async function populateProvincesAndDistricts(workRecords) {
    const recordsArray = Array.isArray(workRecords) ? workRecords : Object.values(workRecords);

    // İl seçeneklerini her zaman doldur
    $('.province-select').each(function () {
        populateProvinces($(this));
    });

    // Event handler'ları ayarla
    setupLocationEventListeners();

    if (recordsArray.length > 0) {
        recordsArray.forEach(record => {
            const date = new Date(record.date);
            const day = date.getDate();

            // İl select'ini doldur
            const provinceSelect = $(`.province-select[data-day="${day}"]`);
            populateProvinces(provinceSelect);

            // İl değerini set et
            setTimeout(() => {
                provinceSelect.val(record.province || '').trigger('change');

                // İlçe select'ini doldur ve değerini set et
                setTimeout(() => {
                    const districtSelect = $(`.district-select[data-day="${day}"]`);
                    if (record.province) {
                        populateDistricts(provinceSelect, districtSelect);
                        setTimeout(() => {
                            districtSelect.val(record.district || '').trigger('change');
                        }, 100);
                    }
                }, 100);
            }, 50);
        });
    }
}

function setupLocationEventListeners() {
    // İl değişikliği event handler'ı - sadece bir kez bağla
    $(document).off('change', '.province-select').on('change', '.province-select', function () {
        const provinceSelect = $(this);
        const day = provinceSelect.data('day');
        const districtSelect = $(`.district-select[data-day="${day}"]`);

        // İlçe select'ini temizle ve doldur
        districtSelect.empty();
        districtSelect.append('<option value="">İlçe seçin</option>');

        if (provinceSelect.val()) {
            populateDistricts(provinceSelect, districtSelect);
        }

        // Select2'yi yeniden başlat
        if (districtSelect.hasClass('select2-hidden-accessible')) {
            districtSelect.select2('destroy');
        }
        districtSelect.select2({
            placeholder: 'İlçe seçin',
            theme: 'bootstrap-5',
            allowClear: true,
            width: '100%'
        });
    });
}

/**
 * Common AJAX Functions
 */
function getRequestConfig(additionalHeaders = {}) {
    return {
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val(),
            ...additionalHeaders
        }
    };
}

function handleAjaxError(error, defaultMessage = 'Bir hata oluştu') {
    console.error('Ajax Error:', error);

    let errorMessage = defaultMessage;
    if (error.responseJSON) {
        if (error.responseJSON.message) {
            errorMessage = error.responseJSON.message;
        } else if (error.responseJSON.errors && error.responseJSON.errors.length > 0) {
            errorMessage = error.responseJSON.errors.join(', ');
        }
    } else if (error.message) {
        errorMessage = error.message;
    }

    toastr.error(errorMessage, 'Hata!');
    return errorMessage;
}

/**
 * Status Badge Helper
 */
function generateStatusBadge(status, statusText) {
    let badgeClass = 'bg-secondary';
    let icon = 'fas fa-clock';

    switch (status) {
        case 0: // Pending
            badgeClass = 'bg-warning text-dark';
            icon = 'fas fa-clock';
            statusText = statusText || 'Bekliyor';
            break;
        case 1: // Approved
            badgeClass = 'bg-success';
            icon = 'fas fa-check';
            statusText = statusText || 'Onaylandı';
            break;
        case 2: // Rejected
            badgeClass = 'bg-danger';
            icon = 'fas fa-times';
            statusText = statusText || 'Reddedildi';
            break;
        default:
            statusText = statusText || 'Yeni';
            badgeClass = 'bg-secondary';
            icon = 'fas fa-circle';
    }

    return `<span class="badge ${badgeClass}">
                <i class="${icon} me-1"></i>${statusText}
            </span>`;
}

/**
 * Initialize all common components
 */
async function initializeCommonComponents() {
    // Set current year and month if not set
    if (!currentYear) {
        currentYear = new Date().getFullYear();
        currentMonth = new Date().getMonth() + 1;
    }

    // Initialize components
    await initializeAllSelects();
    setupExpenseManagement();
    setupAdditionalTimeHandlers();
}

// Auto-initialize when DOM is ready (if jQuery is available)
if (typeof $ !== 'undefined') {
    $(document).ready(function () {
        // Auto-initialize only if explicitly requested
        if (window.autoInitializePuantajCommon !== false) {
            initializeCommonComponents();
        }
    });
}

// Export for manual initialization - Backward compatibility
window.PuantajCommon = {
    Utils: {
        getDayOfWeekTurkish: getDayOfWeekTurkish,
        formatTimeSpan: formatTimeSpan,
        convertMidnightTime: convertMidnightTime,
        validateAndConvertTime: validateAndConvertTime,
        clearExpenseForm: clearExpenseForm,
        showTableLoading: showTableLoading,
        hideTableLoading: hideTableLoading
    },
    Select2Manager: {
        initialize: initializeAllSelects,
        initializeEquipmentSelect: initializeEquipmentSelect,
        initializeLocationSelects: initializeLocationSelects,
        initializeExpenseSelect: initializeExpenseSelect,
        initializeProjectSelect: initializeProjectSelect,
        initializeExcuseReasonSelect: initializeExcuseReasonSelect,
        initializeUserSelect: initializeUserSelect
    },
    ExpenseManager: {
        setup: setupExpenseManagement,
        loadExpensesForDay: loadExpensesForDay,
        displayCurrentExpenses: displayCurrentExpenses,
        addExpenseToDay: addExpenseToDay,
        removeExpenseFromDay: removeExpenseFromDay,
        updateTotalExpense: updateTotalExpense,
        updateExpenseButton: updateExpenseButton
    },
    AdditionalTimeManager: {
        setup: setupAdditionalTimeHandlers
    },
    LocationManager: {
        populateProvincesAndDistricts: populateProvincesAndDistricts,
        setupLocationEventListeners: setupLocationEventListeners
    },
    AjaxHelper: {
        getRequestConfig: getRequestConfig,
        handleError: handleAjaxError
    },
    StatusBadge: {
        generate: generateStatusBadge
    },
    initialize: initializeCommonComponents,
    // Global variables access
    getCurrentYear: () => currentYear,
    getCurrentMonth: () => currentMonth,
    setCurrentYear: (year) => currentYear = year,
    setCurrentMonth: (month) => currentMonth = month,
    getExpenseData: () => expenseData,
    setExpenseData: (data) => expenseData = data,
    getCurrentEditingDay: () => currentEditingDay,
    setCurrentEditingDay: (day) => currentEditingDay = day
};