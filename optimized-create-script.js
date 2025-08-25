@section Scripts {
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-material-datetimepicker/2.7.1/js/bootstrap-material-datetimepicker.min.js"></script>
    <script src="~/js/turkey-locations.js"></script>
    <script src="~/js/puantaj-common.js"></script>

    <script>
        // Page-specific variables
        let workRecordsTable;
        let existingWorkRecords = {};
        let hasApprovedRecords = false;

        // Disable auto-initialization of common components
        window.autoInitializePuantajCommon = false;

        $(document).ready(async function () {
            await initializePage();
        });

        async function initializePage() {
            $('body').addClass('sidebar-hidden');

            // Set current year and month
            PuantajCommon.setCurrentYear(new Date().getFullYear());
            PuantajCommon.setCurrentMonth(new Date().getMonth() + 1);

            $('#year-select').val(PuantajCommon.getCurrentYear());
            $('#month-select').val(PuantajCommon.getCurrentMonth());

            // Initialize common components
            PuantajCommon.initialize();

            // Page-specific event listeners
            setupPageEventListeners();

            // Auto-generate table on page load
            await generateDynamicTable();
        }

        function setupPageEventListeners() {
            $('.btnSaveWorkRecords').on('click', saveWorkRecords);
            $('#year-select, #month-select').on('change', generateDynamicTable);

            // Override expense modal setup for Create page specific behavior
            setupCreatePageExpenseModal();
        }

        function setupCreatePageExpenseModal() {
            // Additional setup specific to Create page
            $('#expenseModal').off('show.bs.modal').on('show.bs.modal', function (event) {
                const button = $(event.relatedTarget);
                const day = button.data('day');
                PuantajCommon.setCurrentEditingDay(day);

                if (day) {
                    const date = new Date(PuantajCommon.getCurrentYear(), PuantajCommon.getCurrentMonth() - 1, day);
                    const dateStr = `${day.toString().padStart(2, '0')}.${PuantajCommon.getCurrentMonth().toString().padStart(2, '0')}.${PuantajCommon.getCurrentYear()} ${PuantajCommon.Utils.getDayOfWeekTurkish(date.getDay())}`;
                    $('#modalDateInfo').text(dateStr);

                    PuantajCommon.ExpenseManager.loadExpensesForDay(day);
                    PuantajCommon.Utils.clearExpenseForm();

                    // Check if this day's record is approved
                    const existingRecord = existingWorkRecords[day];
                    const isApproved = existingRecord && existingRecord.status === 1;

                    toggleExpenseFormElements(isApproved);
                }
            });
        }

        // Create page specific functions
        async function loadExistingWorkRecords(year, month) {
            try {
                const formattedDate = `${year}-${String(month).padStart(2, '0')}-01`;
                const response = await $.ajax({
                    url: '/puantaj/listem',
                    type: 'GET',
                    data: { date: formattedDate },
                    ...PuantajCommon.AjaxHelper.getRequestConfig()
                });

                if (response.isSuccess && response.data && response.data.length > 0) {
                    existingWorkRecords = {};
                    hasApprovedRecords = false;

                    const expenseData = PuantajCommon.getExpenseData();

                    response.data.forEach(record => {
                        const date = new Date(record.date);
                        const day = date.getDate();
                        existingWorkRecords[day] = record;

                        if (record.status === 1) {
                            hasApprovedRecords = true;
                        }

                        if (record.expenses && record.expenses.length > 0) {
                            expenseData[day] = record.expenses;
                        }
                    });

                    PuantajCommon.setExpenseData(expenseData);

                    if (hasApprovedRecords) {
                        showInfoMessage('Bu ay için onaylanmış kayıtlar bulunmaktadır. Onaylanmış kayıtlar düzenlenemez.', 'warning');
                    } else {
                        showInfoMessage('Bu ay için daha önce girilen veriler yüklenmiştir. Değişiklik yapabilirsiniz.', 'info');
                    }

                    return true;
                } else {
                    existingWorkRecords = {};
                    PuantajCommon.setExpenseData({});
                    hasApprovedRecords = false;
                    hideInfoMessage();
                    return false;
                }
            } catch (error) {
                console.error('Mevcut kayıtlar yüklenirken hata:', error);
                existingWorkRecords = {};
                PuantajCommon.setExpenseData({});
                hasApprovedRecords = false;
                hideInfoMessage();
                return false;
            }
        }

        function showInfoMessage(message, type = 'info') {
            const alertClass = type === 'warning' ? 'alert-warning' : 'alert-info';
            const icon = type === 'warning' ? 'fas fa-exclamation-triangle' : 'fas fa-info-circle';

            $('#infoAlert')
                .removeClass('alert-info alert-warning alert-success')
                .addClass(alertClass)
                .removeClass('d-none');

            $('#infoAlert i')
                .removeClass('fas fa-info-circle fas fa-exclamation-triangle fas fa-check-circle')
                .addClass(icon);

            $('#infoMessage').text(message);
        }

        function hideInfoMessage() {
            $('#infoAlert').addClass('d-none');
        }

        function toggleExpenseFormElements(disable) {
            $('#addExpenseForm input, #addExpenseForm select, #addExpenseForm textarea').prop('disabled', disable);
            $('#btnSaveExpenses').prop('disabled', disable);
            $('.remove-expense-btn').prop('disabled', disable);

            if (disable) {
                $('#btnSaveExpenses').html('<i class="fas fa-lock me-1"></i>Onaylandı');
                $('#btnSaveExpenses').removeClass('btn-primary').addClass('btn-secondary');
            } else {
                $('#btnSaveExpenses').html('<i class="fas fa-plus me-1"></i>Masraf Ekle');
                $('#btnSaveExpenses').removeClass('btn-secondary').addClass('btn-primary');
            }
        }

        async function generateDynamicTable() {
            PuantajCommon.Utils.showTableLoading();

            const year = parseInt($('#year-select').val());
            const month = parseInt($('#month-select').val());

            PuantajCommon.setCurrentYear(year);
            PuantajCommon.setCurrentMonth(month);

            await loadExistingWorkRecords(year, month);

            const daysInMonth = new Date(year, month, 0).getDate();
            $('#workRecordsTableBody').empty();

            await new Promise(resolve => setTimeout(resolve, 300));

            // Generate table rows
            const expenseData = PuantajCommon.getExpenseData();
            
            for (let day = 1; day <= daysInMonth; day++) {
                const date = new Date(year, month - 1, day);
                const dayOfWeek = PuantajCommon.Utils.getDayOfWeekTurkish(date.getDay());
                const formattedDate = `${day.toString().padStart(2, '0')}.${month.toString().padStart(2, '0')}.${year} ${dayOfWeek}`;

                const existingRecord = existingWorkRecords[day];
                const isApproved = existingRecord && existingRecord.status === 1;
                const isPending = existingRecord && existingRecord.status === 0;
                const isRejected = existingRecord && existingRecord.status === 2;

                const defaultValues = {
                    isWeekend: existingRecord?.isWeekend || false,
                    startTime: existingRecord ? PuantajCommon.Utils.formatTimeSpan(existingRecord.startTime) : '08:00',
                    endTime: existingRecord ? PuantajCommon.Utils.formatTimeSpan(existingRecord.endTime) : '17:00',
                    project: existingRecord?.project || '',
                    equipment: existingRecord?.equipment || '',
                    province: existingRecord?.province || '',
                    district: existingRecord?.district || '',
                    hasBreakfast: existingRecord?.hasBreakfast || false,
                    hasLunch: existingRecord?.hasLunch || false,
                    hasDinner: existingRecord?.hasDinner || false,
                    hasNightMeal: existingRecord?.hasNightMeal || false
                };

                let statusBadge = PuantajCommon.StatusBadge.generate(null, 'Yeni');
                if (isApproved) {
                    statusBadge = PuantajCommon.StatusBadge.generate(1);
                } else if (isPending) {
                    statusBadge = PuantajCommon.StatusBadge.generate(0);
                } else if (isRejected) {
                    statusBadge = PuantajCommon.StatusBadge.generate(2);
                }

                const disabledAttr = isApproved ? 'disabled' : '';

                const row = createTableRow(day, formattedDate, defaultValues, statusBadge, disabledAttr);
                $('#workRecordsTableBody').append(row);

                // Update expense button if needed
                if (expenseData[day] && expenseData[day].length > 0) {
                    PuantajCommon.ExpenseManager.updateExpenseButton(day);
                }
            }

            await PuantajCommon.LocationManager.populateProvincesAndDistricts(existingWorkRecords);

            PuantajCommon.Select2Manager.initialize();
            PuantajCommon.TimePicker.initialize();

            updateSaveButtonsState();
            PuantajCommon.Utils.hideTableLoading();
        }

        function createTableRow(day, formattedDate, defaultValues, statusBadge, disabledAttr) {
            return `
                <tr data-day="${day}" data-record-id="${existingWorkRecords[day]?.id || ''}" >
                    <td class="date-cell">
                        <span class="badge bg-info">${formattedDate}</span>
                    </td>
                    <td>
                        <div class="form-check d-flex justify-content-center">
                            <input class="form-check-input weekend-checkbox" type="checkbox" id="weekend_${day}"
                                   ${defaultValues.isWeekend ? 'checked' : ''} ${disabledAttr}>
                        </div>
                    </td>
                    <td>
                        <div class="input-group">
                            <input type="text" class="form-control time-input start-time" value="${defaultValues.startTime}"
                                   data-day="${day}" placeholder="08:00" ${disabledAttr}>
                            <span class="input-group-text"><i class="fas fa-clock"></i></span>
                        </div>
                    </td>
                    <td>
                        <div class="input-group">
                            <input type="text" class="form-control time-input end-time" value="${defaultValues.endTime}"
                                   data-day="${day}" placeholder="17:00" ${disabledAttr}>
                            <span class="input-group-text"><i class="fas fa-clock"></i></span>
                        </div>
                    </td>
                    <td>
                        <input type="text" class="form-control project-input" placeholder="Proje adı"
                               data-day="${day}" value="${defaultValues.project}" ${disabledAttr}>
                    </td>
                    <td>
                        <select class="form-select form-select-sm equipment-select" data-day="${day}" ${disabledAttr}>
                            <option value="">Ekipman seçin</option>
                            <option value="excavator" ${defaultValues.equipment === 'excavator' ? 'selected' : ''}>Ekskavatör</option>
                            <option value="bulldozer" ${defaultValues.equipment === 'bulldozer' ? 'selected' : ''}>Buldozer</option>
                            <option value="loader" ${defaultValues.equipment === 'loader' ? 'selected' : ''}>Yükleyici</option>
                            <option value="truck" ${defaultValues.equipment === 'truck' ? 'selected' : ''}>Kamyon</option>
                            <option value="crane" ${defaultValues.equipment === 'crane' ? 'selected' : ''}>Vinç</option>
                            <option value="other" ${defaultValues.equipment === 'other' ? 'selected' : ''}>Diğer</option>
                        </select>
                    </td>
                    <td>
                        <select class="form-select form-select-sm province-select" data-day="${day}" ${disabledAttr}>
                        </select>
                    </td>
                    <td>
                        <select class="form-select form-select-sm district-select" data-day="${day}" ${disabledAttr}>
                        </select>
                    </td>
                    <td>
                        <button type="button" class="btn btn-primary expense-modal-btn w-100"
                                data-day="${day}" data-bs-toggle="modal" data-bs-target="#expenseModal" >
                            <i class="fas fa-receipt me-1"></i>
                            <span class="expense-count" data-day="${day}">0</span> Masraf
                        </button>
                    </td>
                    <td>
                        <div class="meal-section">
                            <div class="meal-item">
                                <div class="form-check">
                                    <input class="form-check-input meal-checkbox" type="checkbox" id="s_${day}"
                                           data-meal="S" data-day="${day}" ${defaultValues.hasBreakfast ? 'checked' : ''} ${disabledAttr}>
                                    <label class="form-check-label" for="s_${day}">S</label>
                                </div>
                            </div>
                            <div class="meal-item">
                                <div class="form-check">
                                    <input class="form-check-input meal-checkbox" type="checkbox" id="o_${day}"
                                           data-meal="O" data-day="${day}" ${defaultValues.hasLunch ? 'checked' : ''} ${disabledAttr}>
                                    <label class="form-check-label" for="o_${day}">Ö</label>
                                </div>
                            </div>
                            <div class="meal-item">
                                <div class="form-check">
                                    <input class="form-check-input meal-checkbox" type="checkbox" id="a_${day}"
                                           data-meal="A" data-day="${day}" ${defaultValues.hasDinner ? 'checked' : ''} ${disabledAttr}>
                                    <label class="form-check-label" for="a_${day}">A</label>
                                </div>
                            </div>
                            <div class="meal-item">
                                <div class="form-check">
                                    <input class="form-check-input meal-checkbox" type="checkbox" id="g_${day}"
                                           data-meal="G" data-day="${day}" ${defaultValues.hasNightMeal ? 'checked' : ''} ${disabledAttr}>
                                    <label class="form-check-label" for="g_${day}">G</label>
                                </div>
                            </div>
                        </div>
                    </td>
                    <td>
                        ${statusBadge}
                    </td>
                </tr>
            `;
        }

        function updateSaveButtonsState() {
            if (hasApprovedRecords) {
                $('.btnSaveWorkRecords').prop('disabled', true)
                    .html('<i class="fas fa-lock me-2"></i>Onaylanmış kayıtlar var')
                    .removeClass('btn-info')
                    .addClass('btn-secondary');
            } else {
                $('.btnSaveWorkRecords').prop('disabled', false)
                    .html('<i class="fas fa-save me-2"></i>Kaydet')
                    .removeClass('btn-secondary')
                    .addClass('btn-info');
            }
        }

        async function saveWorkRecords() {
            if (hasApprovedRecords) {
                toastr.warning('Onaylanmış kayıtlar bulunduğu için değişiklik yapılamaz!', 'Uyarı!');
                return;
            }

            const workRecords = [];
            const expenseData = PuantajCommon.getExpenseData();

            $('#workRecordsTableBody tr').each(function () {
                const day = $(this).data('day');
                const recordId = $(this).data('record-id') || null;
                const isWeekend = $(this).find('.weekend-checkbox').is(':checked');
                const startTime = $(this).find('.start-time').val();
                const endTime = $(this).find('.end-time').val();
                const project = $(this).find('.project-input').val();
                const equipment = $(this).find('.equipment-select').val();
                const province = $(this).find('.province-select').val();
                const district = $(this).find('.district-select').val();

                const dayExpenses = expenseData[day] || [];
                const expenses = dayExpenses.map(exp => ({
                    expense: exp.expense || exp.type,
                    description: exp.description || null,
                    amount: exp.amount,
                    receiptImageUrl: null
                }));

                const hasBreakfast = $(this).find('.meal-checkbox[data-meal="S"]').is(':checked');
                const hasLunch = $(this).find('.meal-checkbox[data-meal="O"]').is(':checked');
                const hasDinner = $(this).find('.meal-checkbox[data-meal="A"]').is(':checked');
                const hasNightMeal = $(this).find('.meal-checkbox[data-meal="G"]').is(':checked');
                const formattedDate = `${PuantajCommon.getCurrentYear()}-${String(PuantajCommon.getCurrentMonth()).padStart(2, '0')}-${String(day).padStart(2, '0')}`;

                workRecords.push({
                    Id: recordId,
                    Date: formattedDate,
                    IsWeekend: isWeekend,
                    StartTime: startTime,
                    EndTime: endTime,
                    Project: project,
                    Equipment: equipment,
                    Province: province,
                    District: district,
                    HasBreakfast: hasBreakfast,
                    HasLunch: hasLunch,
                    HasDinner: hasDinner,
                    HasNightMeal: hasNightMeal,
                    Expenses: expenses
                });
            });

            try {
                $('.btnSaveWorkRecords').prop('disabled', true)
                    .html('<i class="fas fa-spinner fa-spin me-2"></i>Kaydediliyor...');

                const response = await $.ajax({
                    url: '/puantaj/toplu-ekle',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(workRecords),
                    ...PuantajCommon.AjaxHelper.getRequestConfig()
                });

                if (response.isSuccess) {
                    toastr.success(response.message, 'Başarılı!');
                    await generateDynamicTable();
                } else {
                    toastr.error(response.message, 'Hata!');
                }
            } catch (error) {
                PuantajCommon.AjaxHelper.handleError(error, 'Puantaj kaydedilirken bir hata oluştu.');
            } finally {
                updateSaveButtonsState();
            }
        }
    </script>
}