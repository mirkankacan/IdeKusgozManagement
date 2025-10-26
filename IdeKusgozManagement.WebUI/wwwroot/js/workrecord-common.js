// ===== UTILITY FUNCTIONS =====
function getDayOfWeekTurkish(day) {
    const days = ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'];
    return days[day];
}

function formatTimeSpan(timeSpan) {
    if (!timeSpan || timeSpan == null) return '';
    let formattedTime = '';
    if (typeof timeSpan === 'string' && timeSpan.includes(':')) {
        formattedTime = timeSpan.substring(0, 5);
    } else if (typeof timeSpan === 'string') {
        const parts = timeSpan.split(':');
        if (parts.length >= 2) {
            formattedTime = `${parts[0].padStart(2, '0')}:${parts[1].padStart(2, '0')}`;
        }
    }
    if (formattedTime === '00:00') {
        formattedTime = '23:59';
    }
    return formattedTime;
}

function validateAndConvertTime(timeValue) {
    if (!timeValue || typeof timeValue !== 'string') return timeValue;
    timeValue = timeValue.trim();
    if (timeValue === '00:00') {
        return '23:59';
    }
    return timeValue;
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

// ===== SELECT2 MANAGEMENT =====
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

async function initializeDailyStatusSelect() {
    try {
        const reasons = [
            { value: 'Çalışıyor', text: 'Çalışıyor' },
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

        $('.daily-status-select').each(function () {
            const currentSelect = $(this);
            const currentValue = currentSelect.data('current-value');

            currentSelect.empty();
            currentSelect.append('<option value=""></option>');

            reasons.forEach(reason => {
                let isSelected = false;

                // Eğer daha önceden bir değer seçilmişse onu kullan
                if (currentValue && currentValue !== '') {
                    isSelected = currentValue === reason.value;
                } else {
                    // Eğer hiç değer seçilmemişse "Çalışıyor" varsayılan olsun
                    isSelected = reason.value === 'Çalışıyor';
                }

                currentSelect.append(`<option value="${reason.value}" ${isSelected ? 'selected' : ''}>${reason.text}</option>`);
            });
        });

        $('.daily-status-select').select2({
            placeholder: 'Gün durumunu seçiniz',
            theme: 'bootstrap-5',
            allowClear: true,
            width: '100%'
        });

    } catch (error) {
        console.error('İzin sebep listesi yüklenirken hata:', error);
        toastr.error('İzin sebep listesi yüklenirken hata oluştu', 'Hata!');
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

// ===== EXPENSE MANAGEMENT =====
function clearExpenseForm() {
    $('#expenseForm')[0].reset();
    $('#expenseForm').removeClass('was-validated');
    $('#expense-select').val('').trigger('change');
}

function updateTotalExpense(day) {
    const expenses = expenseData[day] || [];
    const total = expenses.reduce((sum, expense) => sum + expense.amount, 0);

    const formattedTotal = new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency: 'TRY',
        minimumFractionDigits: 2
    }).format(total);
    $('#totalExpenseAmount').text(formattedTotal);

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

    if (count > 0) {
        const formattedTotal = new Intl.NumberFormat('tr-TR', {
            style: 'currency',
            currency: 'TRY',
            minimumFractionDigits: 2
        }).format(total);
        button.find('.expense-count').html(`${count} Masraf<br/><small>${formattedTotal}</small>`);
    } else {
        button.find('.expense-count').html('0 Masraf<br/><small>₺0,00</small>');
    }

    if (count > 0) {
        button.removeClass('btn-outline-primary').addClass('btn-success');
    } else {
        button.removeClass('btn-success').addClass('btn-outline-primary');
    }
}

// ===== LOCATION MANAGEMENT =====
async function populateProvincesAndDistricts(workRecords) {
    const recordsArray = Array.isArray(workRecords) ? workRecords : Object.values(workRecords);

    $('.province-select').each(function () {
        populateProvinces($(this));
    });

    setupLocationEventListeners();

    if (recordsArray.length > 0) {
        recordsArray.forEach(record => {
            const date = new Date(record.date);
            const day = date.getDate();
            const provinceSelect = $(`.province-select[data-day="${day}"]`);
            populateProvinces(provinceSelect);

            setTimeout(() => {
                provinceSelect.val(record.province || '').trigger('change');
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
    $(document).off('change', '.province-select').on('change', '.province-select', function () {
        const provinceSelect = $(this);
        const day = provinceSelect.data('day');
        const districtSelect = $(`.district-select[data-day="${day}"]`);

        districtSelect.empty();
        districtSelect.append('<option value="">İlçe seçin</option>');

        if (provinceSelect.val()) {
            populateDistricts(provinceSelect, districtSelect);
        }

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

// ===== STATUS BADGE =====
function generateStatusBadge(status, statusText) {
    let badgeClass = 'bg-secondary';
    let icon = 'fas fa-clock';

    switch (status) {
        case 0:
            badgeClass = 'bg-warning text-dark';
            icon = 'fas fa-clock';
            statusText = statusText || 'Bekliyor';
            break;
        case 1:
            badgeClass = 'bg-success';
            icon = 'fas fa-check';
            statusText = statusText || 'Onaylandı';
            break;
        case 2:
            badgeClass = 'bg-danger';
            icon = 'fas fa-times';
            statusText = statusText || 'Reddedildi';
            break;
        default:
            statusText = statusText || 'Yeni';
            badgeClass = 'bg-secondary';
            icon = 'fas fa-circle';
    }

    return `<span class="badge ${badgeClass}"><i class="${icon} me-1"></i>${statusText}</span>`;
}

// ===== NIGHT MEAL ELIGIBILITY CHECK =====
function isTimeRangeOverlappingNight(startTime, endTime) {
    if (!startTime || !endTime) return false;

    const start = parseTimeToMinutes(startTime);
    const end = parseTimeToMinutes(endTime);
    const nightStart = 1 * 60; // 01:00 = 60 dakika
    const nightEnd = 5 * 60;   // 05:00 = 300 dakika

    // Eğer başlangıç ve bitiş aynı gün içindeyse
    if (start < end) {
        return (start < nightEnd && end > nightStart);
    }
    // Eğer gece yarısını geçiyorsa (örneğin 23:00-07:00)
    else {
        return (start < nightEnd || end > nightStart);
    }
}

function parseTimeToMinutes(timeString) {
    const [hours, minutes] = timeString.split(':').map(Number);
    return hours * 60 + minutes;
}

function checkNightMealEligibility(day) {
    const startTime = $(`.start-time[data-day="${day}"]`).val();
    const endTime = $(`.end-time[data-day="${day}"]`).val();
    const additionalStartTime = $(`.additional-start-time[data-day="${day}"]`).val();
    const additionalEndTime = $(`.additional-end-time[data-day="${day}"]`).val();

    const gCheckbox = $(`#g_${day}`);

    // Eğer onaylanmış kayıt varsa G checkbox'u disabled kalacak
    const existingRecord = typeof existingWorkRecords !== 'undefined' && existingWorkRecords[day];
    const isApproved = existingRecord && existingRecord.status === 1;
    if (isApproved) {
        gCheckbox.prop('disabled', true);
        return;
    }

    // Saat aralıklarını kontrol et
    const timeRanges = [];

    if (startTime && endTime) {
        timeRanges.push({ start: startTime, end: endTime });
    }

    if (additionalStartTime && additionalEndTime) {
        timeRanges.push({ start: additionalStartTime, end: additionalEndTime });
    }

    // 01:00-05:00 aralığını kapsayan herhangi bir saat aralığı var mı kontrol et
    const hasNightTime = timeRanges.some(range => {
        return isTimeRangeOverlappingNight(range.start, range.end);
    });

    if (hasNightTime) {
        gCheckbox.prop('disabled', false);
    } else {
        gCheckbox.prop('disabled', true);
        gCheckbox.prop('checked', false);
    }
}

