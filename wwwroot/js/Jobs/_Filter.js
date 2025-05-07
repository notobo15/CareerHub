function formatCurrencyVND(number) {
    return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

class FilterStateManager {
    constructor() {
        this.state = {
            jobLevels: [],
            workingModels: [],
            industries: [],
            companyTypes: [],
            salary: { type: 'any', min: 0, max: 0 },
            province: "all",
            key: "",
            page: 1
        };
        this.subscribers = [];
    }

    updateState(partialState) {
        this.state = { ...this.state, ...partialState };
        this.notify();

    }

    getState() {
        return this.state;
    }

    subscribe(callback) {
        this.subscribers.push(callback);
    }

    notify() {
        for (const cb of this.subscribers) {
            cb(this.state);
        }
    }

    resetState() {
        this.state = {
            jobLevels: [],
            workingModels: [],
            industries: [],
            companyTypes: [],
            salary: { type: 'all', min: 0, max: 0 },
            province: "all",
            key: "",
            page: 1
        };
        this.notify();
    }
}
const filterManager = new FilterStateManager();

$(document).ready(function () {


    const dropdownJob = new DropdownFilter('.inlineJobLevel', { defaultLabel: 'Cấp bậc' });
    const dropdownWorking = new DropdownFilter('.inlineWorking', { defaultLabel: 'Hình thức' });
    const dropdownSalary = new DropdownFilter('.inlineSalary', { defaultLabel: 'Mức lương' });
    const dropdownIndustry = new DropdownFilter('.inlineIndustry', {
        defaultLabel: 'Ngành nghề',
        enableSearch: true,
        searchInputSelector: '.industry-search',
        searchableItemSelector: '.items label.icheckbox',
        noResultSelector: '.noResult'
    });

    // Khởi tạo Modal và liên kết
    const modal = new FilterModal('#filterModal', {
        jobLevels: dropdownJob,
        workingModels: dropdownWorking,
        salary: dropdownSalary,
        industries: dropdownIndustry
    });
    //dropdownJob.setLinkedModal(modal);
    //dropdownWorking.setLinkedModal(modal);
    //dropdownIndustry.setLinkedModal(modal);
    //dropdownSalary.setLinkedModal(modal);
    // Đồng bộ ngược từ DropdownFilter → Modal
    dropdownJob.setOnChange((data) => modal.setFilterValues({ jobLevels: data.checkboxes }));
    dropdownWorking.setOnChange((data) => modal.setFilterValues({ workingModels: data.checkboxes }));
    dropdownIndustry.setOnChange((data) => modal.setFilterValues({ industries: data.checkboxes }));
    dropdownSalary.setOnChange((data) => {
        console.log(data)
        modal.setFilterValues({ salary: data.salary })
    });

});

// -----------------------------------

function fetchJobListByFilters() {
    const selectedLevels = [];
    const selectedWorkingModels = [];
    const selectedIndustries = [];

    // Lấy cấp bậc
    $('input[name="job_level_names[]"]:checked').each(function () {
        selectedLevels.push($(this).val());
    });

    // Lấy hình thức làm việc
    $('input[name="working_models[]"]:checked').each(function () {
        selectedWorkingModels.push($(this).val());
    });

    // Lấy lĩnh vực
    $('input[name="company_industry_ids[]"]:checked').each(function () {
        selectedIndustries.push($(this).val());
    });

    // Lấy mức lương
    let minSalary = parseInt($('#salary-min').val());
    let maxSalary = parseInt($('#salary-max').val());

    let salaryType = 'any';
    if (!isNaN(minSalary) && !isNaN(maxSalary)) {
        salaryType = 'range';
    } else if (!isNaN(minSalary)) {
        salaryType = 'over';
    } else if (!isNaN(maxSalary)) {
        salaryType = 'under';
    }

    minSalary = !isNaN(minSalary) ? minSalary * 1_000_000 : 0;
    maxSalary = !isNaN(maxSalary) ? maxSalary * 1_000_000 : 0;

    const salaryData = {
        type: salaryType,
        min: minSalary,
        max: maxSalary
    };

    $.ajax({
        url: '/jobs/filter',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            province: "all",
            key: "",
            page: 1,
            jobLevels: selectedLevels,
            workingModels: selectedWorkingModels,
            industries: selectedIndustries,
            salary: salaryData
        }),
        beforeSend: function () {
            $('.search-result-wrapper').html('<div class="text-center p-5">Đang tải dữ liệu...</div>');
        },
        success: function (result) {
            $('.search-result-wrapper').html(result);
            feather.replace();
        },
        error: function () {
            console.error('❌ Lỗi khi tìm kiếm công việc.');
        }
    });
}


class DropdownFilter {
    constructor(rootSelector, options = {}) {
        this.$root = $(rootSelector);
        this.$itag = this.$root.find('.itag');
        this.$dropdownMenu = this.$root.find('.dropdown-menu');
        this.$labelText = this.$root.find('.labelText');
        this.$chevronIcon = this.$root.find('.chevronDownIcon');
        this.$clearIcon = this.$root.find('.clearIcon');
        this.$checkboxes = this.$root.find('input[type="checkbox"]');
        this.$radioButtons = this.$root.find('input[type="radio"]');
        this.$applyButton = this.$root.find('.ibtn');
        this.$inputMin = this.$root.find('#salary-min');
        this.$inputMax = this.$root.find('#salary-max');
        this.onChangeCallback = null;
        this.options = Object.assign({
            defaultLabel: 'Lọc',
            enableSearch: false,
            searchInputSelector: '.industry-search',
            searchableItemSelector: '.items label.icheckbox',
            noResultSelector: '.noResult'
        }, options);

        this.bindEvents();
        this.bindOutsideClick();
    }
    setOnChange(callback) {
        this.onChangeCallback = callback;
    }
    bindEvents() {
        this.$itag.on('click', (e) => {
            e.stopPropagation();
            if (DropdownFilter.activeInstance && DropdownFilter.activeInstance !== this) {
                DropdownFilter.activeInstance.closeDropdown();
            }
            const isOpen = this.$dropdownMenu.hasClass('show');
            if (!isOpen) {
                this.openDropdown();
                DropdownFilter.activeInstance = this;
            } else {
                this.closeDropdown();
                DropdownFilter.activeInstance = null;
            }
        });

        this.$clearIcon.on('click', (e) => {
            e.stopPropagation();
            this.clearAll();

            if (typeof this.onChangeCallback === 'function') {
                const filters = this.getSelectedFilters();
                this.onChangeCallback(filters);

            }

            fetchJobListByFilters();
        });

        // ✅ Checkbox filter
        this.$checkboxes.on('change', () => {
            this.updateCheckboxState()
            fetchJobListByFilters()
        });

        // ✅ Radio change → cập nhật input nếu là dropdown lương
        this.$radioButtons.on('change', (e) => {
            const label = $(e.target).siblings('label').text().trim();
            if (this.$inputMin.length && this.$inputMax.length) {
                const match = label.match(/(\d+)\s*-\s*(\d+)/);
                if (match) {
                    this.$inputMin.val(match[1]);
                    this.$inputMax.val(match[2]);
                } else if (label.includes('Dưới')) {
                    const max = label.match(/\d+/);
                    this.$inputMin.val('');
                    this.$inputMax.val(max ? max[0] : '');
                } else if (label.includes('Trên')) {
                    const min = label.match(/\d+/);
                    this.$inputMin.val(min ? min[0] : '');
                    this.$inputMax.val('');
                } else {
                    this.$inputMin.val('');
                    this.$inputMax.val('');
                }
            }
        });

        // ✅ Bấm nút áp dụng (radio/input filter)
        this.$applyButton.on('click', () => {
            const min = parseInt(this.$inputMin.val(), 10);
            const max = parseInt(this.$inputMax.val(), 10);

            const isAllSelected = this.$radioButtons.filter('#salary-all').is(':checked');

            if (!isAllSelected) {
                // Nếu KHÔNG chọn "Tất cả" → mới cần kiểm tra min/max
                if (!isNaN(min) && !isNaN(max) && min >= max) {
                    alert('❌ Mức lương tối thiểu phải nhỏ hơn mức lương tối đa.');
                    return; // Stop submit nếu lỗi
                }
            }

            if (!isNaN(min) && !isNaN(max)) {
                this.setLabelAndClose(`${min} - ${max} triệu`);
            }
            else if (!isNaN(min)) {
                this.setLabelAndClose(`Trên ${min} triệu`);
            }
            else if (!isNaN(max)) {
                this.setLabelAndClose(`Dưới ${max} triệu`);
            }
            else {
                const selected = this.$radioButtons.filter(':checked');
                if (selected.length > 0) {
                    const label = selected.siblings('label').text().trim();
                    this.setLabelAndClose(label);
                } else {
                    this.setLabelAndClose(this.options.defaultLabel);
                }
            }

            fetchJobListByFilters();
        });

        if (this.options.enableSearch) {
            this.bindSearch();
        }


        this.$inputMin.add(this.$inputMax).on('input', () => {
            this.selectSalaryAgree();
            this.handleSalaryInputValidation();
        });
    }
    selectSalaryAgree() {
        const $agreeRadio = this.$root.find('input#salary-agree');
        if ($agreeRadio.length) {
            $agreeRadio.prop('checked', true);
        }
    }
    handleSalaryInputValidation() {
        let min = parseInt(this.$inputMin.val(), 10);
        let max = parseInt(this.$inputMax.val(), 10);

        // Nếu nhập âm → disable input luôn
        if ((!isNaN(min) && min < 0) || (!isNaN(max) && max < 0)) {
            this.$inputMin.prop('disabled', true);
            this.$inputMax.prop('disabled', true);
            return;
        }

        // Nếu min >= max → disable input
        if (!isNaN(min) && !isNaN(max) && min >= max) {
            this.$inputMin.prop('disabled', true);
            this.$inputMax.prop('disabled', true);
            return;
        }

        // ✅ Ngược lại dữ liệu đúng → enable input và apply button
        this.$inputMin.prop('disabled', false);
        this.$inputMax.prop('disabled', false);

        // ✅ Set lại range hợp lệ
        if (!isNaN(min)) {
            this.$inputMax.attr('min', min + 1);
        } else {
            this.$inputMax.removeAttr('min');
        }

        if (!isNaN(max)) {
            this.$inputMin.attr('max', max - 1);
        } else {
            this.$inputMin.removeAttr('max');
        }

    }

    bindOutsideClick() {
        $(document).on('click', (e) => {
            const isClickInside = $(e.target).closest(this.$root).length > 0;
            if (!isClickInside) {
                this.closeDropdown();
                if (DropdownFilter.activeInstance === this) {
                    DropdownFilter.activeInstance = null;
                }
            }
        });
    }

    updateCheckboxState() {
        const checked = this.$checkboxes.filter(':checked');
        const values = checked.map(function () {
            return $(this).siblings('span').text().trim();
        }).get();

        const count = values.length;

        if (count > 0) {
            this.$itag.addClass('show selected');
            this.$dropdownMenu.addClass('show');
            this.$chevronIcon.addClass('d-none');
            this.$clearIcon.removeClass('d-none');

            this.$labelText.empty().append(`<span class="label-badge">${values[0]}</span>`);
            if (count > 1) {
                this.$labelText.append(`<span class="label-badge">+${count - 1}</span>`);
            }
        } else {
            this.resetUI();
        }
        if (typeof this.onChangeCallback === 'function') {
            this.onChangeCallback(this.getSelectedFilters());
        }
       
    }

    setLabelAndClose(label) {
        this.$labelText.empty().append(`<span class="label-badge">${label}</span>`);
        this.$itag.addClass('selected');
        this.closeDropdown();
        this.$clearIcon.removeClass('d-none');
        this.$chevronIcon.addClass('d-none');
        if (typeof this.onChangeCallback === 'function') {
            this.onChangeCallback(this.getSelectedFilters());
        }
    }

    clearAll() {
        this.$checkboxes.prop('checked', false);
        this.$radioButtons.prop('checked', false);
        this.$inputMin.val && this.$inputMin.val('');
        this.$inputMax.val && this.$inputMax.val('');
        this.resetUI();
    }

    resetUI() {
        this.$itag.removeClass('selected show');
        this.$dropdownMenu.removeClass('show');
        this.$labelText.text(this.options.defaultLabel);
        this.$clearIcon.addClass('d-none');
        this.$chevronIcon.removeClass('d-none');
    }

    openDropdown() {
        this.$itag.addClass('show');
        this.$dropdownMenu.addClass('show');
    }

    closeDropdown() {
        this.$itag.removeClass('show');
        this.$dropdownMenu.removeClass('show');
    }

    bindSearch() {
        const $input = this.$root.find(this.options.searchInputSelector);
        const $items = this.$root.find(this.options.searchableItemSelector);
        const $noResult = this.$root.find(this.options.noResultSelector);
        $input.on('input', () => {
            const keyword = $input.val().toLowerCase().trim();
            let matchCount = 0;

            $items.each(function () {
                const $item = $(this);
                const text = $item.text().toLowerCase().trim();
                const isMatch = text.includes(keyword);

                if (isMatch) {
                    $item.addClass('d-block').removeClass('d-none');
                    matchCount++;
                } else {
                    $item.removeClass('d-block').addClass('d-none');
                }
            });

            // Show/hide no-result text
            if (matchCount === 0) {
                $noResult.removeClass('d-none');
            } else {
                $noResult.addClass('d-none');
            }

            // Optional: add "is-searching" class
            if (keyword) {
                this.$root.addClass('is-searching');
                this.openDropdown(); // luôn mở nếu đang tìm
            } else {
                this.$root.removeClass('is-searching');
            }
        });
    }
    getSelectedFilters() {
        const data = {
            type: this.$root.attr('class') || '', // ví dụ: inlineSalary
            checkboxes: [],
            radio: null,
            salary: null
        };

        // Lấy checkbox đã chọn
        this.$checkboxes.filter(':checked').each(function () {
            data.checkboxes.push($(this).val());
        });

        // Lấy radio nếu có
        const $selectedRadio = this.$radioButtons.filter(':checked');
        if ($selectedRadio.length) {
            data.radio = $selectedRadio.val();
        }

        // Lấy giá trị input số nếu có (salary)
        if (this.$inputMin.length || this.$inputMax.length) {
            const min = this.$inputMin.val();
            const max = this.$inputMax.val();
            if (min || max) {
                data.salary = {
                    min: min ? Number(min) : null,
                    max: max ? Number(max) : null,
                    value: $selectedRadio?.val()
                };
            }
        }

        return data;
    }

    setFilterValues(values) {
        if (Array.isArray(values.checkboxes)) {
            this.$checkboxes.each(function () {
                const $cb = $(this);
                const val = $cb.val();
                $cb.prop('checked', values.checkboxes.map(x => x.toString()).includes(val.toString()));
            });

            // ✅ Chỉ gọi khi là checkbox-type filter
            this.updateCheckboxState();
        }

        if (values.radio) {
            this.$radioButtons.each(function () {
                const $rb = $(this);
                $rb.prop('checked', $rb.val() === values.radio);
            });
        }

        if (values.salary) {
            const min = values.salary.min != null ? Number(values.salary.min) : null;
            const max = values.salary.max != null ? Number(values.salary.max) : null;
            const radio = values.salary.value || null;

            if (this.$inputMin.length) this.$inputMin.val(min ?? '');
            if (this.$inputMax.length) this.$inputMax.val(max ?? '');

            if (radio) {
                this.$radioButtons.prop('checked', false);
                this.$radioButtons.filter(`[value="${radio}"]`).prop('checked', true);
            }

            // ✅ Cập nhật lại label đúng cách
            if (!isNaN(min) && !isNaN(max)) {
                this.setLabelAndClose(`${min} - ${max} triệu`);
            } else if (!isNaN(min)) {
                this.setLabelAndClose(`Trên ${min} triệu`);
            } else if (!isNaN(max)) {
                this.setLabelAndClose(`Dưới ${max} triệu`);
            } else if (radio) {
                const $label = this.$radioButtons.filter(`[value="${radio}"]`).closest('.form-check').find('label');
                const labelText = $label.text().trim();
                this.setLabelAndClose(labelText || this.options.defaultLabel);
            } else {
                this.setLabelAndClose(this.options.defaultLabel);
            }
        }
    }


    static initAll(selector, options = {}) {
        $(selector).each(function () {
            new DropdownFilter(this, options);
        });
    }
}

class FilterModal {
    constructor(modalSelector, linkedDropdowns = {}) {
        this.$modal = $(modalSelector);
        this.$form = this.$modal.find('form');
        this.$inputs = this.$modal.find('input');
        this.$searchInput = this.$modal.find('.modalIndustry input[type="text"]');
        this.$searchItems = this.$modal.find('.modalIndustry .items label.icheckbox');
        this.$noResult = this.$modal.find('.modalIndustry .noResult');
        this.$resetBtn = this.$modal.find('.clearAll');
        this.linkedDropdowns = linkedDropdowns;
        this.bindEvents();
        this.updateCheckedUI(); // init checked on load
        this.bindSubmitAndReset();
        this.updateCheckedUI();
    }

    bindEvents() {
        // 1. Toggle icon khi chọn checkbox
        this.$inputs.on('change', (e) => {
            const $input = $(e.target);
            if ($input.attr('type') === 'radio') {
                this.handleRadioChange($input);
            } else {
                this.toggleCheckboxUI($input);
            }
        });

        // 2. Tìm kiếm lĩnh vực
        this.$searchInput.on('input', () => this.handleSearch());

        // 3. Reset toàn bộ
        this.$resetBtn.on('click', () => {
            this.$inputs.prop('checked', false);
            this.updateCheckedUI();
        });

        //this.$modal.find('#salary-min, #salary-max').on('input', () => {
        //    this.clearSalarySelections();
        //});

        this.$modal.find('#salary-min, #salary-max').on('input', () => {
            this.selectSalaryAgree(); // tự chọn radio "Thỏa thuận"
            this.handleSalaryInputValidation(); // validate min/max
        });
    }

    selectSalaryAgree() {
        const $agreeRadio = this.$modal.find('input#salary_option_agree');
        const $allRadios = this.$modal.find('input[name="salary"]');

        if ($agreeRadio.length) {
            $allRadios.prop('checked', false); // Bỏ checked tất cả radio trước
            $agreeRadio.prop('checked', true); // Check lại "Thỏa thuận"
        }

        this.updateCheckedUI();
    }

    handleSalaryInputValidation() {
        const $minInput = this.$modal.find('#salary-min');
        const $maxInput = this.$modal.find('#salary-max');
        const min = parseInt($minInput.val(), 10);
        const max = parseInt($maxInput.val(), 10);

        // Nếu nhập âm → disable input luôn
        if ((!isNaN(min) && min < 0) || (!isNaN(max) && max < 0)) {
            $minInput.prop('disabled', true);
            $maxInput.prop('disabled', true);
            return;
        }

        // Nếu min >= max → disable input
        if (!isNaN(min) && !isNaN(max) && min >= max) {
            $minInput.prop('disabled', true);
            $maxInput.prop('disabled', true);
            return;
        }

        // ✅ Ngược lại dữ liệu đúng → enable input và apply button
        $minInput.prop('disabled', false);
        $maxInput.prop('disabled', false);

        // ✅ Set lại range hợp lệ
        if (!isNaN(min)) {
            $maxInput.attr('min', min + 1);
        } else {
            $maxInput.removeAttr('min');
        }

        if (!isNaN(max)) {
            $minInput.attr('max', max - 1);
        } else {
            $minInput.removeAttr('max');
        }
    }


    toggleCheckboxUI($input) {
        const $label = $input.closest('label');
        const isChecked = $input.is(':checked');

        if (isChecked) {
            $label
                .addClass('selected input-checkbox-checked')
                .removeClass('input-checkbox-unchecked');
            $label.find('.plus-icon').addClass('d-none');
            $label.find('.check-icon').removeClass('d-none');
        } else {
            $label
                .removeClass('selected input-checkbox-checked')
                .addClass('input-checkbox-unchecked');
            $label.find('.plus-icon').removeClass('d-none');
            $label.find('.check-icon').addClass('d-none');
        }

    }

    clearSalarySelections() {
        const $salaryInputs = this.$modal.find('input[name="salary"]');

        $salaryInputs.each((_, input) => {
            const $input = $(input);
            $input.prop('checked', false);
            this.toggleCheckboxUI($input);
        });
    }

    updateCheckedUI() {
        this.$inputs.each((_, el) => {
            this.toggleCheckboxUI($(el));
        });
    }

    handleSearch() {
        const keyword = this.$searchInput.val().toLowerCase().trim();
        let matchCount = 0;
        this.$searchItems.each(function () {
            const $item = $(this);
            const text = $item.text().toLowerCase().trim();
            const isMatch = text.includes(keyword);

            if (isMatch) {
                $item.addClass('d-block').removeClass('d-none');
                matchCount++;
            } else {
                $item.removeClass('d-block').addClass('d-none');
            }
        });

        // Show/hide no-result text
        if (matchCount === 0) {
            this.$noResult.removeClass('d-none').addClass('d-block');
        } else {
            this.$noResult.addClass('d-none').removeClass('d-block');
        }

        // Optional: add "is-searching" class
        if (keyword) {
            this.$root.addClass('is-searching');
            this.openDropdown(); // luôn mở nếu đang tìm
        } else {
            this.$root.removeClass('is-searching');
        }
    }

    handleRadioChange($input) {
        const name = $input.attr('name'); // ví dụ salaryOption
        const $allRadios = this.$modal.find(`input[name="${name}"]`);
        const $label = $input.closest('label');
        // Reset tất cả radio cùng name
        $allRadios.each(function () {
            const $radioLabel = $(this).closest('label');
            $radioLabel
                .removeClass('selected input-checkbox-checked')
                .addClass('input-checkbox-unchecked');
            $radioLabel.find('.plus-icon').removeClass('d-none');
            $radioLabel.find('.check-icon').addClass('d-none');
        });

        // Set lại cho radio đang chọn
        $label
            .addClass('selected input-checkbox-checked')
            .removeClass('input-checkbox-unchecked');
        $label.find('.plus-icon').addClass('d-none');
        $label.find('.check-icon').removeClass('d-none');

        // Cập nhật input min/max nếu là mức lương
        const labelText = $label.text().trim();
        const match = labelText.match(/(\d+)\s*-\s*(\d+)/);

        if (match) {
            this.$modal.find('#salary-min').val(match[1]);
            this.$modal.find('#salary-max').val(match[2]);
        } else if (labelText.includes('Dưới')) {
            const max = labelText.match(/\d+/);
            this.$modal.find('#salary-min').val('');
            this.$modal.find('#salary-max').val(max ? max[0] : '');
        } else if (labelText.includes('Trên')) {
            const min = labelText.match(/\d+/);
            this.$modal.find('#salary-min').val(min ? min[0] : '');
            this.$modal.find('#salary-max').val('');
        } else {
            // "Tất cả" hoặc "Thỏa thuận"
            this.$modal.find('#salary-min').val('');
            this.$modal.find('#salary-max').val('');
        }
    }

    getSelectedFilters() {
        const data = {
            jobLevels: [],
            workingModels: [],
            industries: [],
            salary: {
                type: 'none',
                min: null,
                max: null,
                value: null
            },
            companyTypes: []
        };

        // jobLevels[]
        this.$modal.find('input[name="job_level_names[]"]:checked').each((_, el) => {
            data.jobLevels.push($(el).val());
        });

        // workingModels[]
        this.$modal.find('input[name="working_models[]"]:checked').each((_, el) => {
            data.workingModels.push($(el).val());
        });

        // industries[]
        this.$modal.find('input[name="company_industry_ids[]"]:checked').each((_, el) => {
            data.industries.push($(el).val());
        });

        // companyTypes[]
        this.$modal.find('input[name="company_types[]"]:checked').each((_, el) => {
            data.companyTypes.push($(el).val());
        });

        // salary range
        const $selectedRadio = this.$modal.find('input[name="salary"]:checked');


        const min = parseInt(this.$modal.find('#salary-min').val(), 10);
        const max = parseInt(this.$modal.find('#salary-max').val(), 10);

        if (!isNaN(min) || !isNaN(max)) {
            data.salary.type = 'range';
            data.salary.min = isNaN(min) ? 0 : min;
            data.salary.max = isNaN(max) ? 0 : max;
            data.salary.value = $selectedRadio?.val();
        } else {
            data.salary.type = 'none';
        }

        return data;
    }

    bindSubmitAndReset() {
        // "Áp dụng" – lấy dữ liệu
        this.$modal.find('.btnSubmitModel').on('click', (e) => {
            e.preventDefault();
            const filters = this.getSelectedFilters();

            // Đồng bộ lại các DropdownFilter UI
            for (const key in this.linkedDropdowns) {
                const dropdown = this.linkedDropdowns[key];
                if (dropdown && typeof dropdown.setFilterValues === 'function') {
                    const sectionValue = filters[key];

                    console.log("sectionValue", sectionValue)
                    if (sectionValue) {
                        let dropdownData = {};

                        if (['jobLevels', 'workingModels', 'industries', 'companyTypes'].includes(key)) {
                            dropdownData = { checkboxes: sectionValue };
                        } else if (key === 'salary') {
                            dropdownData = { salary: sectionValue };
                        }
                        dropdown.setFilterValues(dropdownData);
                    }
                }
            }

            // ✅ Khắc phục: Gán lại giá trị cho chính modal để giữ trạng thái
            this.setFilterValues(filters);

            console.log('✅ Đồng bộ từ Modal về DropdownFilter:', filters);
        });

        // "Xoá bộ lọc"
        this.$modal.find('.btnReset').on('click', (e) => {
            e.preventDefault();
            this.$inputs.each((_, el) => {
                const $el = $(el);
                if ($el.is(':checkbox') || $el.is(':radio')) {
                    $el.prop('checked', false);
                    this.toggleCheckboxUI($el);
                } else {
                    $el.val('');
                }
            });
        });
    }
    setFilterValues(filters) {

        // 1. Reset tất cả input trước
        this.$modal.find('input[type="checkbox"], input[type="radio"]').prop('checked', false);
        this.$modal.find('#salary-min').val('');
        this.$modal.find('#salary-max').val('');

        // 2. Check lại các giá trị mới

        // Job levels
        filters.jobLevels?.forEach(val => {
            this.$modal.find(`input[name="job_level_names[]"][value="${val}"]`).prop('checked', true);
        });

        // Working models
        filters.workingModels?.forEach(val => {
            this.$modal.find(`input[name="working_models[]"][value="${val}"]`).prop('checked', true);
        });

        // Industries
        filters.industries?.forEach(val => {
            this.$modal.find(`input[name="company_industry_ids[]"][value="${val}"]`).prop('checked', true);
        });

        // Company types
        filters.companyTypes?.forEach(val => {
            this.$modal.find(`input[name="company_types[]"][value="${val}"]`).prop('checked', true);
        });



        if (filters.salary) {
            const salary = filters.salary;

            // Gán input min/max nếu có
            if (salary.min != null) this.$modal.find('#salary-min').val(salary.min);
            if (salary.max != null) this.$modal.find('#salary-max').val(salary.max);

            // Chọn radio theo salary.type
            if (salary.value) {
                this.$modal.find(`input[name="salary"][value="${salary.value}"]`).prop('checked', true);
            }
            
        }

        // 3. Update lại giao diện
        this.updateCheckedUI();
    }
}
