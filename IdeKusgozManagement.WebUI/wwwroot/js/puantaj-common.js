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
const PuantajUtils = {
    // Turkish day names
    getDayOfWeekTurkish: function(day) {
        const days = ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'];
        return days[day];
    },

    // Format TimeSpan to HH:mm
    formatTimeSpan: function(timeSpan) {
        if (!timeSpan) return '';

        // If timeSpan is already in HH:mm format, return as is
        if (typeof timeSpan === 'string' && timeSpan.includes(':')) {
            return timeSpan.substring(0, 5); // Get HH:mm part
        }

        // If timeSpan is in .NET TimeSpan format (e.g., "08:00:00")
        if (typeof timeSpan === 'string') {
            const parts = timeSpan.split(':');
            if (parts.length >= 2) {
                return `${parts[0].padStart(2, '0')}:${parts[1].padStart(2, '0')}`;
            }
        }

        return '';
    },

    // Get expense type text
    getExpenseTypeText: function(type) {
        const types = {
            'yol': 'Yol Masrafı',
            'yemek': 'Yemek Masrafı',
            'konaklama': 'Konaklama Masrafı',
            'yakit': 'Yakıt Masrafı',
            'otopark': 'Otopark Masrafı',
            'diger': 'Diğer Masraf'
        };
        return types[type] || type;
    },

    // Clear expense form
    clearExpenseForm: function() {
        $('#expenseForm')[0].reset();
        $('#expenseForm').removeClass('was-validated');
        $('#ddExpense').val('').trigger('change');
    },

    // Show/hide table loading
    showTableLoading: function() {
        $('#dynamicTableContainer').addClass('table-loading');
        $('#tableLoadingSpinner').show();
        $('#tableContent').hide();
    },

    hideTableLoading: function() {
        $('#dynamicTableContainer').removeClass('table-loading');
        $('#tableLoadingSpinner').hide();
        $('#tableContent').show();
    }
};

/**
 * Time Picker Management
 */
const TimePicker = {
    initialize: function() {
        this.initializeStartTime();
        this.initializeEndTime();
    },

    initializeStartTime: function() {
        $('.start-time:not(.dtp-initialized)').each(function () {
            $(this).addClass('dtp-initialized').bootstrapMaterialDatePicker({
                format: 'HH:mm',
                shortTime: false,
                date: false,
                time: true,
                monthPicker: false,
                yearPicker: false,
                dayPicker: false,
                hourPicker: true,
                minutePicker: true,
                minuteStep: 15,
                clearButton: true,
                nowButton: true,
                switchOnClick: true,
                ampm: false,
                okText: 'Tamam',
                cancelText: 'İptal',
                clearText: 'Temizle',
                nowText: 'Şimdi',
                lang: 'tr',
                currentDate: moment().startOf('day').add(8, 'hours'),
                showTodayBtn: false,
                showClear: true,
                showClose: true
            }).on('change', function () {
                TimePicker.validateTimeInputs($(this));
            });

            $(this).closest('.input-group').find('.fa-clock').parent().on('click', function () {
                $(this).closest('.input-group').find('.start-time').focus();
            });
        });
    },

    initializeEndTime: function() {
        $('.end-time:not(.dtp-initialized)').each(function () {
            $(this).addClass('dtp-initialized').bootstrapMaterialDatePicker({
                format: 'HH:mm',
                shortTime: false,
                date: false,
                time: true,
                monthPicker: false,
                yearPicker: false,
                dayPicker: false,
                hourPicker: true,
                minutePicker: true,
                minuteStep: 15,
                clearButton: true,
                nowButton: true,
                switchOnClick: true,
                ampm: false,
                okText: 'Tamam',
                cancelText: 'İptal',
                clearText: 'Temizle',
                nowText: 'Şimdi',
                lang: 'tr',
                currentDate: moment().startOf('day').add(17, 'hours'),
                showTodayBtn: false,
                showClear: true,
                showClose: true
            }).on('change', function () {
                TimePicker.validateTimeInputs($(this));
            });

            $(this).closest('.input-group').find('.fa-clock').parent().on('click', function () {
                $(this).closest('.input-group').find('.end-time').focus();
            });
        });
    },

    validateTimeInputs: function(changedInput) {
        const row = changedInput.closest('tr');
        const startTimeInput = row.find('.start-time');
        const endTimeInput = row.find('.end-time');

        const startTime = startTimeInput.val();
        const endTime = endTimeInput.val();

        if (startTime && endTime) {
            const startMoment = moment(startTime, 'HH:mm');
            const endMoment = moment(endTime, 'HH:mm');

            if (endMoment.isSameOrBefore(startMoment)) {
                toastr.warning('Bırakma saati başlama saatinden erken olamaz!', 'Uyarı!');

                if (changedInput.hasClass('end-time')) {
                    const newEndTime = startMoment.add(1, 'hour').format('HH:mm');
                    endTimeInput.val(newEndTime);
                }
                else if (changedInput.hasClass('start-time')) {
                    const newEndTime = startMoment.add(1, 'hour').format('HH:mm');
                    endTimeInput.val(newEndTime);
                }
            }
        }
    }
};

/**
 * Select2 Management
 */
const Select2Manager = {
    initialize: function() {
        this.initializeEquipmentSelect();
        this.initializeLocationSelects();
        this.initializeExpenseSelect();
    },

    initializeEquipmentSelect: function() {
        $('.equipment-select').select2({
            placeholder: 'Ekipman seçin',
            theme: 'bootstrap-5',
            allowClear: true,
            width: '100%'
        });
    },

    initializeLocationSelects: function() {
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
    },

    initializeExpenseSelect: function() {
        $('#ddExpense').select2({
            placeholder: 'Masraf türü seçin',
            theme: 'bootstrap-5',
            dropdownParent: $('#expenseModal'),
            width: '100%'
        });
    },

    initializeUserSelect: function() {
        $('.user-select').select2({
            placeholder: 'Kullanıcı seçin',
            theme: 'bootstrap-5',
            width: '100%'
        });
    }
};

/**
 * Expense Management
 */
const ExpenseManager = {
    setup: function() {
        this.setupModal();
        this.setupEventListeners();
    },

    setupModal: function() {
        $('#expenseModal').on('show.bs.modal', function (event) {
            const button = $(event.relatedTarget);
            currentEditingDay = button.data('day');

            if (currentEditingDay) {
                const date = new Date(currentYear, currentMonth - 1, currentEditingDay);
                const dateStr = `${currentEditingDay.toString().padStart(2, '0')}.${currentMonth.toString().padStart(2, '0')}.${currentYear} ${PuantajUtils.getDayOfWeekTurkish(date.getDay())}`;
                $('#modalDateInfo').text(dateStr);

                ExpenseManager.loadExpensesForDay(currentEditingDay);
                PuantajUtils.clearExpenseForm();
            }
        });
    },

    setupEventListeners: function() {
        // Form submit işlemi
        $('#expenseForm').on('submit', function(e) {
            e.preventDefault();

            if (this.checkValidity()) {
                ExpenseManager.addExpenseToDay();
            }

            $(this).addClass('was-validated');
        });

        // Masraf silme butonları
        $(document).on('click', '.remove-expense-btn', function() {
            const expenseIndex = $(this).data('index');
            ExpenseManager.removeExpenseFromDay(expenseIndex);
        });
    },

    loadExpensesForDay: function(day) {
        if (!expenseData[day]) {
            expenseData[day] = [];
        }

        this.displayCurrentExpenses(day);
        this.updateTotalExpense(day);
    },

    displayCurrentExpenses: function(day) {
        const expenses = expenseData[day] || [];
        const expensesList = $('#expenseItemsList');
        const currentExpensesSection = $('#currentExpensesList');

        expensesList.empty();

        if (expenses.length > 0) {
            currentExpensesSection.show();

            expenses.forEach((expense, index) => {
                const expenseTypeText = PuantajUtils.getExpenseTypeText(expense.expense || expense.type);
                
                const expenseItem = `
                    <div class="list-group-item d-flex justify-content-between align-items-center">
                        <div>
                            <h6 class="mb-1">${expenseTypeText}</h6>
                            <p class="mb-1"><strong>${expense.amount.toFixed(2)} ₺</strong></p>
                            ${expense.description ? `<small class="text-muted">${expense.description}</small>` : ''}
                        </div>
                        <button type="button" class="btn btn-sm btn-outline-danger remove-expense-btn"
                                data-index="${index}" title="Masrafı Sil">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                `;
                expensesList.append(expenseItem);
            });
        } else {
            currentExpensesSection.hide();
        }
    },

    addExpenseToDay: function() {
        const expenseType = $('#ddExpense').val();
        const amount = parseFloat($('#txtAmount').val());
        const description = $('#txtDescription').val();
        const receiptFile = $('#fileReceiptImage')[0].files[0];

        if(expenseType == null || expenseType == undefined || isNaN(amount) || amount == null ){
            toastr.warning("Masraf türünü ya da tutarı kontrol ediniz");
            return;
        }

        if (!expenseData[currentEditingDay]) {
            expenseData[currentEditingDay] = [];
        }

        const newExpense = {
            type: expenseType,
            expense: expenseType,
            amount: amount,
            description: description,
            receiptFile: receiptFile,
            receiptName: receiptFile ? receiptFile.name : null,
        };

        expenseData[currentEditingDay].push(newExpense);

        // Listeyi güncelle
        this.displayCurrentExpenses(currentEditingDay);
        this.updateTotalExpense(currentEditingDay);
        this.updateExpenseButton(currentEditingDay);

        // Formu temizle
        PuantajUtils.clearExpenseForm();

        toastr.success('Masraf eklendi!', 'Başarılı!');
    },

    removeExpenseFromDay: function(index) {
        if (expenseData[currentEditingDay] && expenseData[currentEditingDay][index]) {
            expenseData[currentEditingDay].splice(index, 1);

            this.displayCurrentExpenses(currentEditingDay);
            this.updateTotalExpense(currentEditingDay);
            this.updateExpenseButton(currentEditingDay);

            toastr.info('Masraf silindi!', 'Bilgi');
        }
    },

    updateTotalExpense: function(day) {
        const expenses = expenseData[day] || [];
        const total = expenses.reduce((sum, expense) => sum + expense.amount, 0);

        $('#totalExpenseAmount').text(total.toFixed(2) + ' ₺');

        if (total > 0) {
            $('#totalExpenseSection').show();
        } else {
            $('#totalExpenseSection').hide();
        }
    },

    updateExpenseButton: function(day) {
        const expenses = expenseData[day] || [];
        const button = $(`.expense-modal-btn[data-day="${day}"]`);
        const count = expenses.length;

        button.find('.expense-count').text(count);

        if (count > 0) {
            button.removeClass('btn-outline-primary').addClass('btn-success');
        } else {
            button.removeClass('btn-success').addClass('btn-outline-primary');
        }
    }
};

/**
 * Location Management (extends turkey-locations.js)
 */
const LocationManager = {
    populateProvincesAndDistricts: async function(workRecords) {
        const recordsArray = Array.isArray(workRecords) ? workRecords : Object.values(workRecords);
        
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
};

/**
 * Common AJAX Functions
 */
const AjaxHelper = {
    // Common request configuration
    getRequestConfig: function(additionalHeaders = {}) {
        return {
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val(),
                ...additionalHeaders
            }
        };
    },

    // Handle common error responses
    handleError: function(error, defaultMessage = 'Bir hata oluştu') {
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
};

/**
 * Status Badge Helper
 */
const StatusBadge = {
    generate: function(status, statusText) {
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
};

/**
 * Initialize all common components
 */
function initializeCommonComponents() {
    // Set current year and month if not set
    if (!currentYear) {
        currentYear = new Date().getFullYear();
        currentMonth = new Date().getMonth() + 1;
    }

    // Initialize components
    TimePicker.initialize();
    Select2Manager.initialize();
    ExpenseManager.setup();
}

// Auto-initialize when DOM is ready (if jQuery is available)
if (typeof $ !== 'undefined') {
    $(document).ready(function() {
        // Auto-initialize only if explicitly requested
        if (window.autoInitializePuantajCommon !== false) {
            initializeCommonComponents();
        }
    });
}

// Export for manual initialization
window.PuantajCommon = {
    Utils: PuantajUtils,
    TimePicker: TimePicker,
    Select2Manager: Select2Manager,
    ExpenseManager: ExpenseManager,
    LocationManager: LocationManager,
    AjaxHelper: AjaxHelper,
    StatusBadge: StatusBadge,
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