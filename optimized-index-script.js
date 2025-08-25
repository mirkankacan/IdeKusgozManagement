@section Scripts {
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-material-datetimepicker/2.7.1/js/bootstrap-material-datetimepicker.min.js"></script>
    <script src="~/js/turkey-locations.js"></script>
    <script src="~/js/puantaj-common.js"></script>

    <script>
        // Page-specific variables
        let workRecordsTable;

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

            // Page-specific initialization
            await setupIndexPage();
        }

        async function setupIndexPage() {
            // Initialize user select
            PuantajCommon.Select2Manager.initializeUserSelect();

            // Setup page-specific event listeners
            setupPageEventListeners();

            // Load users
            await getAssignedUsers();

            // Show empty table initially
            showEmptyTable();

            // Override expense modal for Index page (read-only)
            setupIndexPageExpenseModal();
        }

        function setupPageEventListeners() {
            $('.btnUpdateWorkRecords').on('click', saveWorkRecords);
            $('#user-select').on('change', handleUserSelection);
            $('#year-select, #month-select').on('change', handleDateChange);
            $('.btn-outline-success').on('click', approveWorkRecords);
            $('.btn-outline-danger').on('click', rejectWorkRecords);
        }

        function setupIndexPageExpenseModal() {
            // Override expense modal for Index page - expenses are read-only
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

                    // Disable expense form in Index page (read-only)
                    disableExpenseForm();
                }
            });

            // Disable add expense functionality in Index page
            $('#expenseForm').off('submit');
        }

        function disableExpenseForm() {
            $('#addExpenseForm input, #addExpenseForm select, #addExpenseForm textarea').prop('disabled', true);
            $('#btnSaveExpenses').prop('disabled', true).html('<i class="fas fa-eye me-1"></i>Görüntüleme Modu');
        }

        function handleUserSelection() {
            const selectedUserId = $('#user-select').val();
            if (selectedUserId) {
                loadWorkRecordsByUser();
            } else {
                clearTableData();
            }
        }

        function handleDateChange() {
            const selectedUserId = $('#user-select').val();
            if (selectedUserId) {
                loadWorkRecordsByUser();
            } else {
                showEmptyTable();
            }
        }

        // Table state management functions
        function showEmptyTable() {
            const emptyMessage = `
                <tr>
                    <td colspan="11" class="text-center py-5">
                        <div class="text-muted">
                            <i class="fas fa-info-circle fa-2x mb-3"></i>
                            <h5>Kullanıcı Seçin</h5>
                            <p>Puantaj verilerini görüntülemek için lütfen bir kullanıcı seçin.</p>
                        </div>
                    </td>
                </tr>
            `;
            $('#workRecordsTableBody').html(emptyMessage);
        }

        function showNoDataMessage(year, month) {
            const noDataMessage = `
                <tr>
                    <td colspan="11" class="text-center py-5">
                        <div class="text-muted">
                            <i class="fas fa-calendar-times fa-2x mb-3 text-warning"></i>
                            <h5>Puantaj Verisi Bulunamadı</h5>
                            <p>Seçili kullanıcının ${month}/${year} ayına ait puantaj kaydı bulunmamaktadır.</p>
                            <small>Personel henüz bu ay için puantaj doldurmamış olabilir.</small>
                        </div>
                    </td>
                </tr>
            `;
            $('#workRecordsTableBody').html(noDataMessage);
        }

        function showErrorMessage() {
            const errorMessage = `
                <tr>
                    <td colspan="10" class="text-center py-5">
                        <div class="text-danger">
                            <i class="fas fa-exclamation-triangle fa-2x mb-3"></i>
                            <h5>Veri Yükleme Hatası</h5>
                            <p>Puantaj verileri yüklenirken bir hata oluştu.</p>
                            <button class="btn btn-outline-primary btn-sm" onclick="loadWorkRecordsByUser()">
                                <i class="fas fa-redo me-1"></i>Tekrar Dene
                            </button>
                        </div>
                    </td>
                </tr>
            `;
            $('#workRecordsTableBody').html(errorMessage);
        }

        function clearTableData() {
            showEmptyTable();
            PuantajCommon.setExpenseData({});
        }

        // Data loading functions
        async function loadWorkRecordsByUser() {
            const selectedUserId = $('#user-select').val();
            const year = parseInt($('#year-select').val());
            const month = parseInt($('#month-select').val());

            if (!selectedUserId) {
                showEmptyTable();
                return;
            }

            try {
                PuantajCommon.Utils.showTableLoading();
                const formattedDate = `${year}-${String(month).padStart(2, '0')}-01`;

                const response = await $.ajax({
                    url: '/puantaj/liste',
                    type: 'GET',
                    data: {
                        date: formattedDate,
                        userId: selectedUserId
                    },
                    ...PuantajCommon.AjaxHelper.getRequestConfig()
                });

                if (response.isSuccess && response.data && response.data.length > 0) {
                    populateTableWithWorkRecords(response.data);
                    toastr.success(`${response.data.length} puantaj kaydı yüklendi`, 'Başarılı!');
                } else {
                    showNoDataMessage(year, month);
                }
            } catch (error) {
                console.error('Puantaj verileri yüklenirken hata:', error);
                showErrorMessage();
                PuantajCommon.AjaxHelper.handleError(error, 'Puantaj verileri yüklenirken bir hata oluştu');
            } finally {
                PuantajCommon.Utils.hideTableLoading();
            }
        }

        function populateTableWithWorkRecords(workRecords) {
            $('#workRecordsTableBody').empty();
            PuantajCommon.setExpenseData({});

            // Sort records by date
            workRecords.sort((a, b) => new Date(a.date) - new Date(b.date));

            const expenseData = PuantajCommon.getExpenseData();

            workRecords.forEach(record => {
                const date = new Date(record.date);
                const day = date.getDate();
                const dayOfWeek = PuantajCommon.Utils.getDayOfWeekTurkish(date.getDay());
                const formattedDate = `${day.toString().padStart(2, '0')}.${(date.getMonth() + 1).toString().padStart(2, '0')}.${date.getFullYear()} ${dayOfWeek}`;

                const startTime = PuantajCommon.Utils.formatTimeSpan(record.startTime);
                const endTime = PuantajCommon.Utils.formatTimeSpan(record.endTime);
                const statusBadge = PuantajCommon.StatusBadge.generate(record.status, record.statusText);

                const row = createIndexTableRow(record, day, formattedDate, startTime, endTime, statusBadge);
                $('#workRecordsTableBody').append(row);

                // Populate expense data
                if (record.expenses && record.expenses.length > 0) {
                    expenseData[day] = record.expenses;
                    PuantajCommon.ExpenseManager.updateExpenseButton(day);
                }
            });

            PuantajCommon.setExpenseData(expenseData);

            // Initialize location selects and populate values
            PuantajCommon.LocationManager.populateProvincesAndDistricts(workRecords);

            // Re-initialize components
            PuantajCommon.Select2Manager.initialize();
            PuantajCommon.TimePicker.initialize();
        }

        function createIndexTableRow(record, day, formattedDate, startTime, endTime, statusBadge) {
            return `
                <tr data-day="${day}" data-record-id="${record.id}">
                    <td class="date-cell">
                        <span class="badge bg-info">${formattedDate}</span>
                    </td>
                    <td>
                        <div class="form-check d-flex justify-content-center">
                            <input class="form-check-input weekend-checkbox" type="checkbox" id="weekend_${day}" ${record.isWeekend ? 'checked' : ''}>
                        </div>
                    </td>
                    <td>
                        <div class="input-group">
                            <input type="text" class="form-control time-input start-time" value="${startTime}" data-day="${day}" placeholder="08:00" readonly>
                            <span class="input-group-text"><i class="fas fa-clock"></i></span>
                        </div>
                    </td>
                    <td>
                        <div class="input-group">
                            <input type="text" class="form-control time-input end-time" value="${endTime}" data-day="${day}" placeholder="17:00" readonly>
                            <span class="input-group-text"><i class="fas fa-clock"></i></span>
                        </div>
                    </td>
                    <td>
                        <input type="text" class="form-control project-input" placeholder="Proje adı" data-day="${day}" value="${record.project || ''}">
                    </td>
                    <td>
                        <select class="form-select form-select-sm equipment-select" data-day="${day}">
                            <option value="">Ekipman seçin</option>
                            <option value="excavator" ${record.equipment === 'excavator' ? 'selected' : ''}>Ekskavatör</option>
                            <option value="bulldozer" ${record.equipment === 'bulldozer' ? 'selected' : ''}>Buldozer</option>
                            <option value="loader" ${record.equipment === 'loader' ? 'selected' : ''}>Yükleyici</option>
                            <option value="truck" ${record.equipment === 'truck' ? 'selected' : ''}>Kamyon</option>
                            <option value="Crane" ${record.equipment === 'Crane' ? 'selected' : ''}>Vinç</option>
                            <option value="other" ${record.equipment === 'other' ? 'selected' : ''}>Diğer</option>
                        </select>
                    </td>
                    <td>
                        <select class="form-select form-select-sm province-select" data-day="${day}">
                            <option value="">İl seçin</option>
                        </select>
                    </td>
                    <td>
                        <select class="form-select form-select-sm district-select" data-day="${day}">
                            <option value="">İlçe seçin</option>
                        </select>
                    </td>
                    <td>
                        <button type="button" class="btn btn-primary expense-modal-btn w-100"
                                data-day="${day}" data-bs-toggle="modal" data-bs-target="#expenseModal">
                            <i class="fas fa-receipt me-1"></i>
                            <span class="expense-count" data-day="${day}">0</span> Masraf
                        </button>
                    </td>
                    <td>
                        <div class="meal-section">
                            <div class="meal-item">
                                <div class="form-check">
                                    <input class="form-check-input meal-checkbox" type="checkbox" id="s_${day}" data-meal="S" data-day="${day}" ${record.hasBreakfast ? 'checked' : ''}>
                                    <label class="form-check-label" for="s_${day}">S</label>
                                </div>
                            </div>
                            <div class="meal-item">
                                <div class="form-check">
                                    <input class="form-check-input meal-checkbox" type="checkbox" id="o_${day}" data-meal="O" data-day="${day}" ${record.hasLunch ? 'checked' : ''}>
                                    <label class="form-check-label" for="o_${day}">Ö</label>
                                </div>
                            </div>
                            <div class="meal-item">
                                <div class="form-check">
                                    <input class="form-check-input meal-checkbox" type="checkbox" id="a_${day}" data-meal="A" data-day="${day}" ${record.hasDinner ? 'checked' : ''}>
                                    <label class="form-check-label" for="a_${day}">A</label>
                                </div>
                            </div>
                            <div class="meal-item">
                                <div class="form-check">
                                    <input class="form-check-input meal-checkbox" type="checkbox" id="g_${day}" data-meal="G" data-day="${day}" ${record.hasNightMeal ? 'checked' : ''}>
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

        // User management functions
        async function getAssignedUsers() {
            try {
                const userSelects = $('.user-select');
                if (!userSelects.length) {
                    console.warn('User select elements not found');
                    return;
                }

                userSelects.prop('disabled', true);

                userSelects.each(function() {
                    const currentSelect = $(this);
                    currentSelect.empty();
                    currentSelect.append('<option value="">Yükleniyor...</option>');

                    if (currentSelect.hasClass('select2-hidden-accessible')) {
                        currentSelect.trigger('change');
                    }
                });

                const response = await $.ajax({
                    url: '/kullanici-yonetimi/liste',
                    type: 'GET',
                    dataType: 'json',
                    ...PuantajCommon.AjaxHelper.getRequestConfig()
                });

                if (response && response.isSuccess && response.data) {
                    userSelects.each(function() {
                        const currentSelect = $(this);
                        currentSelect.empty();
                        currentSelect.append('<option value="">Kullanıcı seçin</option>');

                        response.data.forEach(user => {
                            currentSelect.append(`<option value="${user.id}">${user.fullName}</option>`);
                        });

                        if (currentSelect.hasClass('select2-hidden-accessible')) {
                            currentSelect.trigger('change');
                        }
                    });
                } else {
                    throw new Error(response?.message || 'Atanmış kullanıcılar yüklenemedi');
                }
            } catch (error) {
                console.error('Atanmış kullanıcılar yüklenirken hata:', error);

                $('.user-select').each(function() {
                    const currentSelect = $(this);
                    currentSelect.html('<option value="">Kullanıcılar yüklenemedi</option>');

                    if (currentSelect.hasClass('select2-hidden-accessible')) {
                        currentSelect.trigger('change');
                    }
                });

                PuantajCommon.AjaxHelper.handleError(error, 'Atanmış kullanıcılar yüklenirken hata oluştu');
            } finally {
                $('.user-select').prop('disabled', false);
            }
        }

        // Save and approval functions
        async function saveWorkRecords() {
            const workRecords = [];
            const selectedUserId = $('#user-select').val();

            if (!selectedUserId) {
                toastr.warning('Lütfen bir kullanıcı seçin', 'Uyarı!');
                return;
            }

            const expenseData = PuantajCommon.getExpenseData();

            $('#workRecordsTableBody tr').each(function () {
                if ($(this).find('td').length === 1) return; // Skip message rows

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
                    expense: exp.type || exp.expense,
                    description: exp.description || null,
                    amount: exp.amount,
                    receiptImageUrl: exp.receiptImageUrl || null
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
                    Expenses: expenses,
                    UserId: selectedUserId
                });
            });

            try {
                const response = await $.ajax({
                    url: '/puantaj/toplu-ekle',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(workRecords),
                    ...PuantajCommon.AjaxHelper.getRequestConfig()
                });

                if (response.isSuccess) {
                    toastr.success(response.message, 'Başarılı!');
                    await loadWorkRecordsByUser();
                } else {
                    toastr.error(response.message, 'Hata!');
                }
            } catch (error) {
                PuantajCommon.AjaxHelper.handleError(error, 'Puantaj kaydedilirken bir hata oluştu.');
            }
        }

        async function approveWorkRecords() {
            const selectedUserId = $('#user-select').val();
            const year = parseInt($('#year-select').val());
            const month = parseInt($('#month-select').val());

            if (!selectedUserId) {
                toastr.warning('Lütfen bir kullanıcı seçin', 'Uyarı!');
                return;
            }

            try {
                const result = await Swal.fire({
                    title: 'Puantajı Onayla',
                    text: `Bu kullanıcının ${month}/${year} puantajını onaylamak istediğinizden emin misiniz?`,
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonColor: '#28a745',
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: '<i class="fas fa-check"></i> Evet, Onayla',
                    cancelButtonText: '<i class="fas fa-times"></i> İptal',
                    showLoaderOnConfirm: true,
                    preConfirm: async () => {
                        try {
                            const formattedDate = `${year}-${String(month).padStart(2, '0')}-01`;

                            const response = await $.ajax({
                                url: '/puantaj/toplu-onayla',
                                type: 'POST',
                                data: {
                                    userId: selectedUserId,
                                    date: formattedDate
                                },
                                ...PuantajCommon.AjaxHelper.getRequestConfig()
                            });

                            if (!response.isSuccess) {
                                Swal.showValidationMessage(`Hata: ${response.message}`);
                                throw new Error(response.message);
                            }

                            return response;
                        } catch (error) {
                            const errorMessage = PuantajCommon.AjaxHelper.handleError(error, 'Onaylama başarısız');
                            Swal.showValidationMessage(`Hata: ${errorMessage}`);
                            throw error;
                        }
                    },
                    allowOutsideClick: () => !Swal.isLoading()
                });

                if (result.isConfirmed) {
                    toastr.success('Puantaj başarıyla onaylandı', 'Başarılı!');
                    await loadWorkRecordsByUser();
                }
            } catch (error) {
                console.error('Onaylama hatası:', error);
            }
        }

        async function rejectWorkRecords() {
            const selectedUserId = $('#user-select').val();
            const year = parseInt($('#year-select').val());
            const month = parseInt($('#month-select').val());

            if (!selectedUserId) {
                toastr.warning('Lütfen bir kullanıcı seçin', 'Uyarı!');
                return;
            }

            try {
                const result = await Swal.fire({
                    title: 'Puantajı Reddet',
                    text: `Bu kullanıcının ${month}/${year} puantajını reddetmek istediğinizden emin misiniz?`,
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#dc3545',
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: '<i class="fas fa-times"></i> Evet, Reddet',
                    cancelButtonText: '<i class="fas fa-ban"></i> İptal',
                    showLoaderOnConfirm: true,
                    preConfirm: async () => {
                        try {
                            const formattedDate = `${year}-${String(month).padStart(2, '0')}-01`;

                            const response = await $.ajax({
                                url: '/puantaj/toplu-reddet',
                                type: 'POST',
                                data: {
                                    date: formattedDate,
                                    userId: selectedUserId
                                },
                                ...PuantajCommon.AjaxHelper.getRequestConfig()
                            });

                            if (!response.isSuccess) {
                                Swal.showValidationMessage(`Hata: ${response.message}`);
                                throw new Error(response.message);
                            }

                            return response;
                        } catch (error) {
                            const errorMessage = PuantajCommon.AjaxHelper.handleError(error, 'Reddetme başarısız');
                            Swal.showValidationMessage(`Hata: ${errorMessage}`);
                            throw error;
                        }
                    },
                    allowOutsideClick: () => !Swal.isLoading()
                });

                if (result.isConfirmed) {
                    toastr.success('Puantaj başarıyla reddedildi', 'Başarılı!');
                    await loadWorkRecordsByUser();
                }
            } catch (error) {
                console.error('Reddetme hatası:', error);
            }
        }
    </script>
}