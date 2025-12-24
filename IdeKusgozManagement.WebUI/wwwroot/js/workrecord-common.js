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

        const select2Options = {
            placeholder: 'Masraf türü seçin',
            theme: 'bootstrap-5',
            width: '100%'
        };

        // Eğer expenseModal varsa dropdownParent ekle
        if ($('#expenseModal').length > 0) {
            select2Options.dropdownParent = $('#expenseModal');
        }

        $('#expense-select').select2(select2Options);
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
            width: '100%'
        });

        // Setup project change handler for Garaj/Ofis logic
        setupProjectChangeHandler();
    } catch (error) {
        console.error('Proje listesi yüklenirken hata:', error);
        toastr.error('Proje listesi yüklenirken hata oluştu', 'Hata!');
    }
}

function setupProjectChangeHandler() {
    $(document).off('change', '.project-select').on('change', '.project-select', function () {
        const projectSelect = $(this);
        const day = projectSelect.data('day');
        const selectedProjectName = projectSelect.find('option:selected').text();
        
        if (!day) return;

        const row = $(`tr[data-day="${day}"]`);
        if (!row.length) return;

        const isLocked = String(row.attr('data-is-locked')) === 'true';
        if (isLocked) return;

        const isGarajOrOfis = selectedProjectName && 
            (selectedProjectName.toLowerCase() === 'garaj' || selectedProjectName.toLowerCase() === 'ofis');

        const equipmentSelect = row.find('.equipment-select');
        const provinceSelect = row.find('.province-select');
        const districtSelect = row.find('.district-select');

        if (isGarajOrOfis) {
            // Disable equipment
            equipmentSelect.prop('disabled', true);
            if (equipmentSelect.hasClass('select2-hidden-accessible')) {
                equipmentSelect.val(null).trigger('change.select2');
            }

            // Disable location selects
            provinceSelect.prop('disabled', true);
            districtSelect.prop('disabled', true);

            // Set İzmir and Bornova
            if (!provinceSelect.hasClass('select2-hidden-accessible')) {
                populateProvinces(provinceSelect);
            }
            
            setTimeout(() => {
                provinceSelect.val('İzmir').trigger('change');
                setTimeout(() => {
                    if (provinceSelect.val() === 'İzmir') {
                        populateDistricts(provinceSelect, districtSelect);
                        setTimeout(() => {
                            districtSelect.val('Bornova').trigger('change');
                        }, 100);
                    }
                }, 100);
            }, 50);

            if (provinceSelect.hasClass('select2-hidden-accessible')) {
                provinceSelect.trigger('change.select2');
            }
            if (districtSelect.hasClass('select2-hidden-accessible')) {
                districtSelect.trigger('change.select2');
            }
        } else {
            // Enable equipment
            equipmentSelect.prop('disabled', false);
            if (equipmentSelect.hasClass('select2-hidden-accessible')) {
                equipmentSelect.trigger('change.select2');
            }

            // Enable location selects
            provinceSelect.prop('disabled', false);
            districtSelect.prop('disabled', false);

            if (provinceSelect.hasClass('select2-hidden-accessible')) {
                provinceSelect.trigger('change.select2');
            }
            if (districtSelect.hasClass('select2-hidden-accessible')) {
                districtSelect.trigger('change.select2');
            }
        }
    });
}

async function initializeDailyStatusSelect() {
    try {
        const reasons = [
            { value: 'Çalışıyor', text: 'Çalışıyor' },
            { value: 'Hafta Tatili (DAY OFF)', text: 'Hafta Tatili (DAY OFF)' },
            { value: 'Rapor', text: 'Rapor' },
            { value: 'Ücretsiz İzin', text: 'Ücretsiz İzin' },
            { value: 'Yıllık İzin', text: 'Yıllık İzin' },
            { value: 'Babalık İzni', text: 'Babalık İzni' },
            { value: 'İdari İzin', text: 'İdari İzin (Serbest Zaman İzni)' },
            { value: 'Cenaze İzni', text: 'Cenaze İzni' },
            { value: 'Evlilik İzni', text: 'Evlilik İzni' },
            { value: 'Süt İzni', text: 'Süt İzni' },
            { value: 'Resmi Tatil', text: 'Resmi Tatil' }
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
        width: '100%'
    });

    $('.district-select').select2({
        placeholder: 'İlçe seçin',
        theme: 'bootstrap-5',
        width: '100%'
    });
}

const MEAL_ENABLED_STATUSES = [
    'Çalışıyor',
    'Resmi Tatil',
    'Hafta Tatili',
    'Hafta Tatili (DAY OFF)'
];

function shouldDisableMealsForStatus(status) {
    if (!status) {
        return false;
    }
    const normalizedStatus = status.toString().trim().toLowerCase();
    return !MEAL_ENABLED_STATUSES.some(item => item.toLowerCase() === normalizedStatus);
}

function setMealCheckboxState(row, shouldDisable) {
    if (!row || !row.length) {
        return;
    }

    const day = row.data('day');

    row.find('.meal-checkbox').each(function () {
        const checkbox = $(this);
        const mealType = checkbox.data('meal');
        if (mealType === 'G') {
            checkbox.prop('disabled', true);
            if (shouldDisable) {
                checkbox.prop('checked', false);
            }
        } else {
            if (shouldDisable) {
                checkbox.prop('checked', false).prop('disabled', true);
            } else {
                checkbox.prop('disabled', false);
            }
        }
    });

    if (!shouldDisable) {
        checkNightMealEligibility(day);
    } else {
        row.find(`#g_${day}`).prop('checked', false).prop('disabled', true);
    }
}

// Helper functions for consistent disabled styling
function applyDisabledStyle(element) {
    if (element && element.length) {
        element.css({
            'pointer-events': 'none',
            'opacity': '0.6',
            'cursor': 'not-allowed'
        });
    }
}

function removeDisabledStyle(element) {
    if (element && element.length) {
        element.css({
            'pointer-events': '',
            'opacity': '',
            'cursor': ''
        });
    }
}

function updateRowFieldsForStatus(day, status) {
    if (!day) {
        return;
    }

    const row = $(`tr[data-day="${day}"]`);
    if (!row.length) {
        return;
    }

    const isLocked = String(row.attr('data-is-locked')) === 'true';
    if (isLocked) {
        return;
    }

    // Check status conditions
    const isWorking = status === 'Çalışıyor';
    const isDayOff = status === 'Hafta Tatili (DAY OFF)';

    // For meals: if status is not 'Çalışıyor', always disable meals
    // Otherwise, use the existing meal logic
    const disableMeals = !isWorking || shouldDisableMealsForStatus(status);
    setMealCheckboxState(row, disableMeals);
    
    // Get all fields that should be disabled/enabled
    const startTimeInput = row.find('.start-time');
    const endTimeInput = row.find('.end-time');
    const additionalStartTimeInput = row.find('.additional-start-time');
    const additionalEndTimeInput = row.find('.additional-end-time');
    const projectSelect = row.find('.project-select');
    const equipmentSelect = row.find('.equipment-select');
    const provinceSelect = row.find('.province-select');
    const districtSelect = row.find('.district-select');
    const descriptionTextarea = row.find('.description-textarea');
    const internalTransportCheckbox = row.find('.internal-transport-checkbox');
    const expenseModalBtn = row.find('.expense-modal-btn');
    const travelExpenseAmount = row.find('.travel-expense-amount');
    const mealCheckboxLabels = row.find('.meal-checkbox').closest('.form-check-label');

    if (isWorking) {
        // Enable all fields (but respect existing disabled state from date-too-old or other rules)
        const isDateTooOld = String(row.attr('data-is-date-too-old')) === 'true';
        if (!isDateTooOld) {
            startTimeInput.prop('disabled', false);
            endTimeInput.prop('disabled', false);
            if (additionalStartTimeInput.length) additionalStartTimeInput.prop('disabled', false);
            if (additionalEndTimeInput.length) additionalEndTimeInput.prop('disabled', false);
            projectSelect.prop('disabled', false);
            equipmentSelect.prop('disabled', false);
            provinceSelect.prop('disabled', false);
            districtSelect.prop('disabled', false);
            descriptionTextarea.prop('disabled', false);
            internalTransportCheckbox.prop('disabled', false);
            expenseModalBtn.prop('disabled', false);
            travelExpenseAmount.prop('disabled', false);

            // Re-enable all inputs, textareas, and buttons visually
            removeDisabledStyle(startTimeInput);
            removeDisabledStyle(endTimeInput);
            if (additionalStartTimeInput.length) removeDisabledStyle(additionalStartTimeInput);
            if (additionalEndTimeInput.length) removeDisabledStyle(additionalEndTimeInput);
            removeDisabledStyle(descriptionTextarea);
            removeDisabledStyle(internalTransportCheckbox);
            removeDisabledStyle(expenseModalBtn);
            removeDisabledStyle(travelExpenseAmount);
            removeDisabledStyle(mealCheckboxLabels);

            // Handle Select2 dropdowns - re-enable containers
            const select2Selects = [projectSelect, equipmentSelect, provinceSelect, districtSelect];
            select2Selects.forEach(select => {
                if (select.hasClass('select2-hidden-accessible')) {
                    select.trigger('change.select2');
                    const container = select.next('.select2-container');
                    if (container.length) {
                        container.removeClass('select2-container-disabled');
                        removeDisabledStyle(container);
                    }
                } else {
                    // For non-Select2 selects, remove style directly
                    removeDisabledStyle(select);
                }
            });
        }
    } else if (isDayOff) {
        // For "Hafta Tatili (DAY OFF)": disable only time inputs, enable other fields
        const isDateTooOld = String(row.attr('data-is-date-too-old')) === 'true';
        if (!isDateTooOld) {
            // Disable time inputs
            startTimeInput.prop('disabled', true);
            endTimeInput.prop('disabled', true);
            if (additionalStartTimeInput.length) additionalStartTimeInput.prop('disabled', true);
            if (additionalEndTimeInput.length) additionalEndTimeInput.prop('disabled', true);
            applyDisabledStyle(startTimeInput);
            applyDisabledStyle(endTimeInput);
            if (additionalStartTimeInput.length) applyDisabledStyle(additionalStartTimeInput);
            if (additionalEndTimeInput.length) applyDisabledStyle(additionalEndTimeInput);

            // Enable other fields
            projectSelect.prop('disabled', false);
            equipmentSelect.prop('disabled', false);
            provinceSelect.prop('disabled', false);
            districtSelect.prop('disabled', false);
            descriptionTextarea.prop('disabled', false);
            internalTransportCheckbox.prop('disabled', false);
            expenseModalBtn.prop('disabled', false);
            travelExpenseAmount.prop('disabled', false);

            // Re-enable other fields visually
            removeDisabledStyle(descriptionTextarea);
            removeDisabledStyle(internalTransportCheckbox);
            removeDisabledStyle(expenseModalBtn);
            removeDisabledStyle(travelExpenseAmount);
            removeDisabledStyle(mealCheckboxLabels);

            // Handle Select2 dropdowns - re-enable containers
            const select2Selects = [projectSelect, equipmentSelect, provinceSelect, districtSelect];
            select2Selects.forEach(select => {
                if (select.hasClass('select2-hidden-accessible')) {
                    select.trigger('change.select2');
                    const container = select.next('.select2-container');
                    if (container.length) {
                        container.removeClass('select2-container-disabled');
                        removeDisabledStyle(container);
                    }
                } else {
                    // For non-Select2 selects, remove style directly
                    removeDisabledStyle(select);
                }
            });
        }
    } else {
        // Disable all fields when status is not 'Çalışıyor' or 'Hafta Tatili (DAY OFF)'
        startTimeInput.prop('disabled', true);
        endTimeInput.prop('disabled', true);
        if (additionalStartTimeInput.length) additionalStartTimeInput.prop('disabled', true);
        if (additionalEndTimeInput.length) additionalEndTimeInput.prop('disabled', true);
        projectSelect.prop('disabled', true);
        equipmentSelect.prop('disabled', true);
        provinceSelect.prop('disabled', true);
        districtSelect.prop('disabled', true);
        descriptionTextarea.prop('disabled', true);
        internalTransportCheckbox.prop('disabled', true);
        expenseModalBtn.prop('disabled', true);
        travelExpenseAmount.prop('disabled', true);

        // Apply disabled style to all inputs, textareas, checkboxes, and buttons
        applyDisabledStyle(startTimeInput);
        applyDisabledStyle(endTimeInput);
        if (additionalStartTimeInput.length) applyDisabledStyle(additionalStartTimeInput);
        if (additionalEndTimeInput.length) applyDisabledStyle(additionalEndTimeInput);
        applyDisabledStyle(descriptionTextarea);
        applyDisabledStyle(internalTransportCheckbox);
        applyDisabledStyle(expenseModalBtn);
        applyDisabledStyle(travelExpenseAmount);
        applyDisabledStyle(mealCheckboxLabels);

        // Handle Select2 dropdowns - disable the container as well
        const select2Selects = [projectSelect, equipmentSelect, provinceSelect, districtSelect];
        select2Selects.forEach(select => {
            if (select.hasClass('select2-hidden-accessible')) {
                select.trigger('change.select2');
                const container = select.next('.select2-container');
                if (container.length) {
                    container.addClass('select2-container-disabled');
                    applyDisabledStyle(container);
                }
            } else {
                // For non-Select2 selects, apply style directly
                applyDisabledStyle(select);
            }
        });
    }
}

function applyDailyStatusRulesForAllRows() {
    $('#workRecordsTableBody tr').each(function () {
        const row = $(this);
        const day = row.data('day');
        if (!day) {
            return;
        }
        const statusSelect = row.find('.daily-status-select');
        const status = statusSelect.val() || statusSelect.data('current-value') || '';
        updateRowFieldsForStatus(day, status);
    });
}

function applyProjectRulesForAllRows() {
    $('#workRecordsTableBody tr').each(function () {
        const row = $(this);
        const day = row.data('day');
        if (!day) {
            return;
        }
        const projectSelect = row.find('.project-select');
        const selectedProjectName = projectSelect.find('option:selected').text();
        
        const isLocked = String(row.attr('data-is-locked')) === 'true';
        if (isLocked) return;

        const isGarajOrOfis = selectedProjectName && 
            (selectedProjectName.toLowerCase() === 'garaj' || selectedProjectName.toLowerCase() === 'ofis');

        const equipmentSelect = row.find('.equipment-select');
        const provinceSelect = row.find('.province-select');
        const districtSelect = row.find('.district-select');

        if (isGarajOrOfis) {
            // Disable equipment
            equipmentSelect.prop('disabled', true);
            if (equipmentSelect.hasClass('select2-hidden-accessible')) {
                equipmentSelect.trigger('change.select2');
            }

            // Disable location selects
            provinceSelect.prop('disabled', true);
            districtSelect.prop('disabled', true);

            // Set İzmir and Bornova if not already set
            if (provinceSelect.val() !== 'İzmir') {
                if (!provinceSelect.hasClass('select2-hidden-accessible')) {
                    populateProvinces(provinceSelect);
                }
                
                setTimeout(() => {
                    provinceSelect.val('İzmir').trigger('change');
                    setTimeout(() => {
                        if (provinceSelect.val() === 'İzmir') {
                            populateDistricts(provinceSelect, districtSelect);
                            setTimeout(() => {
                                if (districtSelect.val() !== 'Bornova') {
                                    districtSelect.val('Bornova').trigger('change');
                                }
                            }, 100);
                        }
                    }, 100);
                }, 50);
            } else if (districtSelect.val() !== 'Bornova') {
                populateDistricts(provinceSelect, districtSelect);
                setTimeout(() => {
                    districtSelect.val('Bornova').trigger('change');
                }, 100);
            }

            if (provinceSelect.hasClass('select2-hidden-accessible')) {
                provinceSelect.trigger('change.select2');
            }
            if (districtSelect.hasClass('select2-hidden-accessible')) {
                districtSelect.trigger('change.select2');
            }
        } else {
            // Enable equipment
            equipmentSelect.prop('disabled', false);
            if (equipmentSelect.hasClass('select2-hidden-accessible')) {
                equipmentSelect.trigger('change.select2');
            }

            // Enable location selects
            provinceSelect.prop('disabled', false);
            districtSelect.prop('disabled', false);

            if (provinceSelect.hasClass('select2-hidden-accessible')) {
                provinceSelect.trigger('change.select2');
            }
            if (districtSelect.hasClass('select2-hidden-accessible')) {
                districtSelect.trigger('change.select2');
            }
        }
    });
}

$(document).on('change', '.daily-status-select', function () {
    const day = $(this).data('day');
    const status = $(this).val();
    updateRowFieldsForStatus(day, status);
});

// ===== EXPENSE MANAGEMENT =====
function clearExpenseForm() {
    // Formdan validation class'ını kaldır
    $('#expenseForm').removeClass('was-validated');
    
    // Tüm inputlardan validation class'larını kaldır
    $('#expense-select').removeClass('is-valid is-invalid');
    $('#txtAmount').removeClass('is-valid is-invalid');
    $('#txtDescription').removeClass('is-valid is-invalid');
    $('#fileReceipt').removeClass('is-valid is-invalid');
    
    // Select2 validation'ı temizle
    const select2Container = $('#expense-select').parent().find('.select2-selection');
    if (select2Container.length) {
        select2Container.removeClass('is-valid is-invalid');
    }

    // Form değerlerini temizle
    $('#expenseForm')[0].reset();
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
            width: '100%'
        });
    });
}

// ===== STATUS BADGE =====
function generateStatusBadge(status, statusText) {
    let badgeClass = 'bg-secondary';
    let icon = 'fas fa-clock';
    
    // Convert status to number if it's a string, handle null/undefined
    const statusNum = status === null || status === undefined ? null : parseInt(status, 10);
    
    // Check if conversion was successful (not NaN)
    if (statusNum !== null && !isNaN(statusNum)) {
        switch (statusNum) {
            case 0:
                badgeClass = 'bg-warning text-dark';
                icon = 'fas fa-clock';
                statusText = statusText || 'Beklemede';
                break;
            case 1:
                badgeClass = 'bg-success';
                icon = 'fas fa-check';
                statusText = statusText || 'Şef Onayladı';
                break;
            case 2:
                badgeClass = 'bg-info';
                icon = 'fas fa-check';
                statusText = statusText || 'Yönetici Onayladı';
                break;
            case 3:
                badgeClass = 'bg-danger';
                icon = 'fas fa-times';
                statusText = statusText || 'Şef Reddetti';
                break;
            case 4:
                badgeClass = 'bg-danger';
                icon = 'fas fa-times';
                statusText = statusText || 'Yönetici Reddetti';
                break;
            default:
                statusText = statusText || 'Yeni';
                badgeClass = 'bg-secondary';
                icon = 'fas fa-circle';
        }
    } else {
        // If status is null, undefined, or invalid, show as "Yeni"
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

    // Eğer onaylanmış kayıt varsa G checkbox'u disabled kalacak - Check existingWorkRecords first
    const existingRecord = typeof existingWorkRecords !== 'undefined' && existingWorkRecords[day];
    const isApproved = existingRecord && existingRecord.status === 1;
    if (isApproved) {
        gCheckbox.prop('disabled', true);
        return;
    }

    // Also check DOM for status badge (for Index page)
    const row = $(`tr[data-day="${day}"]`);
    if (row.length > 0) {
        const statusBadge = row.find('.badge');
        if (statusBadge.hasClass('bg-success')) {
            gCheckbox.prop('disabled', true);
            return;
        }
    }

    // Saat aralıklarını kontrol et
    const timeRanges = [];

    if (startTime && endTime) {
        timeRanges.push({ start: startTime, end: endTime });
    }

    if (additionalStartTime && additionalEndTime) {
        timeRanges.push({ start: additionalStartTime, end: additionalEndTime });
    }

    // Şoför-Yük Taşıma için özel kontrol: 19:00'dan sonra gece yemeği
    const isDriverDuty = typeof window.departmentDutyName !== 'undefined' && 
                         window.departmentDutyName === 'Şoför-Yük Taşıma';
    let hasNightTime = false;
    
    if (isDriverDuty) {
        // Şoför-Yük Taşıma için: 19:00'dan sonra başlayan veya biten saat aralıkları
        hasNightTime = timeRanges.some(range => {
            const start = parseTimeToMinutes(range.start);
            const end = parseTimeToMinutes(range.end);
            const eveningStart = 19 * 60; // 19:00 = 1140 dakika
            
            // Eğer başlangıç ve bitiş aynı gün içindeyse
            if (start < end) {
                return start >= eveningStart || end >= eveningStart;
            }
            // Eğer gece yarısını geçiyorsa (örneğin 23:00-07:00)
            else {
                return start >= eveningStart || end >= eveningStart;
            }
        });
    } else {
        // Normal kontrol: 01:00-05:00 aralığını kapsayan herhangi bir saat aralığı var mı kontrol et
        hasNightTime = timeRanges.some(range => {
            return isTimeRangeOverlappingNight(range.start, range.end);
        });
    }

    if (hasNightTime) {
        gCheckbox.prop('disabled', false);
    } else {
        gCheckbox.prop('disabled', true);
        gCheckbox.prop('checked', false);
    }
}

// ===== TIME INPUT VALIDATION =====
function isValidTimeString(timeStr) {
    if (!timeStr || timeStr.length !== 5 || !timeStr.includes(':')) {
        return false;
    }
    
    const parts = timeStr.split(':');
    const hours = parseInt(parts[0], 10);
    const minutes = parseInt(parts[1], 10);
    
    return (hours >= 0 && hours <= 23) && (minutes >= 0 && minutes <= 59);
}

function formatTimeInput(value) {
    if (!value) return '';
    
    // Sadece rakam karakterlerine izin ver
    value = value.replace(/[^0-9]/g, '');
    
    // Maksimum 4 karakter (0000) çünkü : otomatik eklenecek
    if (value.length > 4) {
        value = value.substring(0, 4);
    }
    
    let result = '';
    
    for (let i = 0; i < value.length; i++) {
        const char = parseInt(value[i]);
        const pos = i;
        
        // İlk karakter (saat onlar basamağı): 0-2
        if (pos === 0) {
            if (char <= 2) {
                result += value[i];
            }
            // Geçersizse tamamen durdur
            else {
                break;
            }
        }
        // İkinci karakter (saat birler basamağı): İlk karaktere göre
        else if (pos === 1) {
            if (result[0] === '2') {
                // Eğer ilk rakam 2 ise, ikinci rakam sadece 0-3 olabilir
                if (char <= 3) {
                    result += value[i];
                } else {
                    // Geçersizse durdur
                    break;
                }
            } else if (result[0] === '0' || result[0] === '1') {
                // İlk rakam 0 veya 1 ise, ikinci rakam 0-9 olabilir
                result += value[i];
            }
        }
        // Üçüncü karakter (dakika onlar basamağı): 0-5
        else if (pos === 2) {
            if (char <= 5) {
                result += value[i];
            } else {
                // Geçersizse durdur
                break;
            }
        }
        // Dördüncü karakter (dakika birler basamağı): 0-9
        else if (pos === 3) {
            result += value[i];
        }
    }
    
    // : karakterini pozisyona göre ekle ve formatla
    if (result.length === 0) {
        return '';
    } else if (result.length === 1) {
        return result;
    } else if (result.length === 2) {
        return result + ':';
    } else if (result.length === 3) {
        return result.substring(0, 2) + ':' + result.substring(2);
    } else {
        return result.substring(0, 2) + ':' + result.substring(2);
    }
}

// ===== TIME INPUT HANDLERS =====
function setupTimeInputHandlers() {
    // Additional time button toggle
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

    // Focus event
    $(document).on('focus', '.time-input, .additional-start-time, .additional-end-time', function () {
        const input = $(this)[0];
        const value = input.value;
        if (value === ':') {
            input.value = '';
            input.setSelectionRange(0, 0);
        }
    });

    // Keydown event - Backspace/Delete ile : karakterini atlama
    $(document).on('keydown', '.time-input, .additional-start-time, .additional-end-time', function (e) {
        const input = $(this)[0];
        const currentValue = input.value;
        const cursorPos = input.selectionStart;
        const keyCode = e.keyCode || e.which;
        
        // Backspace işlemi için özel kontrol
        if (keyCode === 8) {
            // Eğer sadece saat kısmı var (örn: "23:") ve cursor sonunda ise
            if (currentValue.length === 3 && currentValue[2] === ':' && cursorPos === 3) {
                e.preventDefault();
                // : karakterini atla ve son rakamı sil
                const hourPart = currentValue.substring(0, 2);
                input.value = hourPart.substring(0, 1);
                input.setSelectionRange(1, 1);
                return;
            }
        }
    });

    // Keypress event - Sadece rakam karakterlerine izin ver
    $(document).on('keypress', '.time-input, .additional-start-time, .additional-end-time', function (e) {
        const key = e.key || String.fromCharCode(e.keyCode);
        const keyCode = e.keyCode || e.which;
        
        // Sadece 0-9 karakterlerine izin ver
        const isDigit = (key >= '0' && key <= '9') || (keyCode >= 48 && keyCode <= 57);
        const isDelete = keyCode === 8 || keyCode === 46 || key === 'Backspace' || key === 'Delete';
        const isTab = keyCode === 9 || key === 'Tab';
        
        if (!isDigit && !isDelete && !isTab) {
            e.preventDefault();
            return false;
        }
    });

    // Keyup event
    $(document).on('keyup', '.time-input, .additional-start-time, .additional-end-time', function (e) {
        const input = $(this)[0];
        const value = input.value;
        if ((e.keyCode === 8 || e.keyCode === 46) && value === ':') {
            input.value = '';
            input.setSelectionRange(0, 0);
        }
    });

    // Click event
    $(document).on('click', '.time-input, .additional-start-time, .additional-end-time', function () {
        const input = $(this)[0];
        const value = input.value;
        if (value === ':') {
            input.value = '';
            input.setSelectionRange(0, 0);
        }
    });

    // Input event - Format and validate
    $(document).on('input', '.time-input, .additional-start-time, .additional-end-time', function () {
        const input = $(this)[0];
        let value = $(this).val();
        
        // Eğer : varsa, onu kaldır ve sadece rakamları işle
        const numericValue = value.replace(/:/g, '');
        
        // Eğer kullanıcı silme işlemi yapıyorsa
        if (numericValue.length === 0) {
            input.value = '';
            $(this).data('oldValue', '');
            return;
        }
        
        // Eski değerin numeric versiyonu
        const oldNumericValue = $(this).data('oldValue') || '';
        $(this).data('oldValue', numericValue);
        
        // Sadece rakamları formatla
        value = formatTimeInput(numericValue);
        
        $(this).val(value);
        
        // Eğer 00:00 yazıldıysa anında 23:59'a çevir
        if (value === '00:00') {
            $(this).val('23:59');
            input.setSelectionRange(5, 5);
            return;
        }
        
        // Cursor'u her zaman en sona al
        setTimeout(() => {
            input.setSelectionRange(value.length, value.length);
        }, 0);
    });

    // Blur event - Final validation
    $(document).on('blur', '.time-input, .additional-start-time, .additional-end-time', function () {
        let value = $(this).val();
        
        if (value === '00:00') {
            $(this).val('23:59');
            return;
        }
        
        // If value exists but is not complete, pad it
        if (value && value.length < 5 && value.includes(':')) {
            const parts = value.split(':');
            if (parts[0] && !parts[1]) {
                $(this).val(parts[0] + ':00');
                return;
            }
        }
        
        // Validate final value
        if (value && !isValidTimeString(value)) {
            // Reset to empty or default
            if (value.length >= 3) {
                // Try to fix partial values
                const parts = value.split(':');
                if (parts[0] && parts[1]) {
                    const hours = parseInt(parts[0], 10) || 0;
                    const minutes = parseInt(parts[1], 10) || 0;
                    
                    if (hours > 23) {
                        parts[0] = '23';
                    }
                    if (minutes > 59) {
                        parts[1] = '59';
                    }
                    
                    $(this).val(`${parts[0].padStart(2, '0')}:${parts[1].padStart(2, '0')}`);
                }
            }
        }
    });
}

// ===== HOLIDAY MANAGEMENT =====
let holidayData = {};

async function loadHolidays(year) {
    try {
        const response = await $.ajax({
            url: `/tatil/${year}/yili`,
            type: 'GET',
            dataType: 'json',
            timeout: 10000
        });
        if (response?.isSuccess && Array.isArray(response.data)) {
            holidayData = {};
            response.data.forEach(holiday => {
                if (holiday.date && holiday.name) {
                    const isoString = holiday.date.iso;
                    if (!isoString) {
                        console.warn('No ISO date found for holiday:', holiday);
                        return;
                    }
                    const holidayDate = new Date(isoString);
                    if (isNaN(holidayDate.getTime())) {
                        console.warn('Invalid date for holiday:', holiday);
                        return;
                    }
                    const day = holidayDate.getDate();
                    const month = holidayDate.getMonth() + 1;
                    const key = `${month}-${day}`;

                    holidayData[key] = {
                        name: holiday.name,
                        date: holiday.date,
                        primaryType: holiday.primary_type
                    };
                }
            });
            return true;
        } else {
            console.warn('Geçersiz response formatı:', response);
            holidayData = {};
            return false;
        }
    } catch (error) {
        console.error('Tatil verileri yüklenirken hata:', error);
        holidayData = {};
        return false;
    }
}

function checkIfHoliday(day, month, year) {
    if (!holidayData) return false;
    const key = `${month}-${day}`;
    const holiday = holidayData[key];
    return holiday || false;
}

function isFullHoliday(day, month, year) {
    const holiday = checkIfHoliday(day, month, year);
    return holiday && holiday.primaryType === 'National holiday';
}

function isHalfDayHoliday(day, month, year) {
    const holiday = checkIfHoliday(day, month, year);
    return holiday && holiday.primaryType === 'Half Day';
}

// Get holiday row class for table styling
function getHolidayRowClass(day, month, year, isSunday = false) {
    const isFullHolidayDay = isFullHoliday(day, month, year);
    const isHalfHolidayDay = isHalfDayHoliday(day, month, year);
    
    let rowClass = '';
    // Sunday or full holiday = red background
    if (isSunday || isFullHolidayDay) {
        rowClass = 'table-danger'; // Red - Full holiday or Sunday
    } else if (isHalfHolidayDay) {
        rowClass = 'table-warning'; // Yellow - Half-day holiday
    }
    
    return rowClass;
}

