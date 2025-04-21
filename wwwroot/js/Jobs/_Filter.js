function formatCurrencyVND(number) {
    return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}
$(document).ready(function () {

    class FilterManager {
        constructor() {
            this.filterState = {
                jobLevels: [],
                workingModels: [],
                industries: [],
                salary: {
                    type: "range",
                    min: 0,
                    max: 5000000
                },
                companyTypes: []
            };

            this.initEvents();
        }

        initEvents() {
            this.handleCheckboxSync();
            this.handleSalarySync();
            this.handleFilterClear();
            this.handleModalToggle();
            this.handleDropdownToggle();
            this.handleBtnSubmitModel();
            this.handleSubmitLog();
            this.handleSearchIndustries();
        }

        handleSubmitLog() {
            $(document).on('click', "button[type='submit']", (e) => {
                console.log('Current filter state:', this.filterState);
            });
        }

        fetchData() {
            $.ajax({
                url: "/jobs/filter", // Controller xử lý
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ province: "all", key: "", page: 1, ...this.filterState }),
                success: function (result) {
                    $(".search-result-wrapper").html(result);
                    feather.replace();
                },
                error: function () {
                    console.error("Không thể tải dữ liệu chi tiết công việc.");
                }
            });

            console.log("Salary filter:", this.filterState);
        }

        handleSearchIndustries() {
            $(".industry .form-control").on("input", function () {
                let searchValue = $(this).val().toLowerCase(); // Lấy giá trị nhập vào (chuyển thành chữ thường)
                let $items = $(".industry .items label"); // Tất cả các label
                let $noResult = $(".industry .noResult"); // Thông báo không có kết quả
                let matchCount = 0;

                $items.each(function () {
                    let text = $(this).text().toLowerCase(); // Lấy text trong label
                    if (text.includes(searchValue)) {
                        $(this).removeClass("d-none"); // Hiển thị nếu khớp từ khóa
                        matchCount++;
                    } else {
                        $(this).addClass("d-none"); // Ẩn nếu không khớp
                    }
                });

                // Hiển thị hoặc ẩn thông báo "Không tìm thấy kết quả"
                if (matchCount === 0) {
                    $noResult.removeClass("d-none");
                } else {
                    $noResult.addClass("d-none");
                }
            });
        }

        handleDropdownToggle() {
            $(document).on('click', '.dropdown', function (event) {
                event.stopPropagation(); // Prevent bubbling

                let $this = $(this);
                let $dropdownModel = $this.find(".itag");
                let $dropdownMenu = $this.find(".dropdown-menu");

                let isOpen = $dropdownModel.hasClass("show");

                $(".dropdown .itag, .dropdown .dropdown-menu").removeClass("show"); // Close others

                if (!isOpen) {
                    $dropdownModel.addClass("show");
                    $dropdownMenu.addClass("show");
                }
            });

            $(document).on('click', '.dropdown-menu', function (event) {
                event.stopPropagation(); // Prevent close when clicking inside menu
            });

            $(document).on('click', function () {
                $(".dropdown .itag, .dropdown .dropdown-menu").removeClass("show"); // Close all when clicking outside
            });
        }

        handleCheckboxSync() {
            $(document).on('change', "input[type='checkbox']", (e) => {
                let name = $(e.target).attr('name');
                let value = $(e.target).val();
                let isChecked = $(e.target).is(':checked');

                $(`input[name='${name}'][value='${value}']`).prop('checked', isChecked);
                this.updateFilterState(name, value, isChecked);
                FilterManager.updateFilterLabels(name);
                FilterManager.checkAnyFilterSelected();
                //this.fetchData()
            });

            //$(".clearIcon").click(function (event) {
            //    event.stopPropagation(); // Ngăn chặn click lan ra ngoài
            //    let $dropdown = $(this).closest(".dropdown");

            //    $dropdown.find("input[type='checkbox']").prop("checked", false).trigger("change"); // Bỏ chọn tất cả và kích hoạt sự kiện change
            //    $dropdown.find("input[type='radio']").prop("checked", false).trigger("change"); // Bỏ chọn tất cả và kích hoạt sự kiện change
            //});
            $(".clearIcon").click((event) => {
                event.stopPropagation();

                let $dropdown = $(event.currentTarget).closest(".dropdown");

                // Nếu là dropdown lương
                if ($dropdown.hasClass("salary")) {
                    // Reset radio
                    $dropdown.find("input[type='radio']").prop("checked", false);
                    $dropdown.find("#salary-all").prop("checked", true);

                    $dropdown.find("#salary-range-section").hide()

                    $dropdown.removeClass("show")

                    // Reset range slider
                    $dropdown.find("input[name='salary_ranges[]'].min").val(0);
                    $dropdown.find("input[name='salary_ranges[]'].max").val(50000000);

                    // Cập nhật filterState
                    filterManager.filterState.salary = { min: 0, max: 50000000, type: "all" };

                    // Cập nhật UI hiển thị lương
                    FilterManager.updateSalaryDisplay(0, 50000000, $dropdown);

                    // Cập nhật label lương
                    $dropdown.find(".dropdown-model span:first").text("Mức lương");
                    $dropdown.find(".dropdown-model").removeClass("selected");
                    $dropdown.find(".clearIcon").addClass("d-none");
                    $dropdown.find(".chevronDownIcon").removeClass("d-none");

                    // Đóng dropdown
                    $dropdown.find(".dropdown-model").removeClass("show");
                    $dropdown.find(".dropdown-menu").removeClass("show");

                    // Gọi lại fetchData
                    filterManager.fetchData();

                    // Cập nhật hiển thị nút "Xóa lọc"
                    FilterManager.checkAnyFilterSelected();
                } else {
                    // Các dropdown khác (checkbox, như bạn đã có)
                    $dropdown.find("input[type='checkbox']").prop("checked", false).trigger("change");
                    //$dropdown.find("input[type='radio']").prop("checked", false).trigger("change");
                }
            });

            $(".modal .itag").on('click', function (e) {
                e.preventDefault();
                let $input = $(this).find('input');
                $input.prop('checked', !$input.is(':checked'));
                if ($input.is(':checked')) {
                    $(this).removeClass("input-checkbox-unchecked")
                    $(this).addClass("selected input-checkbox-checked")
                    $(this).prop('checked', false);
                    $(this).find('.plus-icon').addClass('d-none');
                    $(this).find('.check-icon').removeClass('d-none');
                } else {
                    $(this).removeClass("selected input-checkbox-checked")
                    $(this).addClass("input-checkbox-unchecked")
                    $(this).prop('checked', true);
                    $(this).find('.plus-icon').removeClass('d-none');
                    $(this).find('.check-icon').addClass('d-none');
                }
            });
        }

        updateFilterState(name, value, isChecked) {
            console.log(name, value, isChecked)
            let map = {
                "job_level_names[]": 'jobLevels',
                "working_models[]": 'workingModels',
                "company_industry_ids[]": 'industries',
                "company_types[]": 'companyTypes'
            };
            let key = map[name];
            if (!key) return;

            if (isChecked) {
                if (!this.filterState[key].includes(value)) this.filterState[key].push(value);
            } else {
                this.filterState[key] = this.filterState[key].filter(v => v !== value);
            }
        }

        handleSalarySync() {
            let tempMinSalary = 0;
            let tempMaxSalary = 50000000;


            $('input[name="salaryOption"]').on('change', function (e) {
                const $rangeContainer = $(e.target).closest(".dropdown-menu");
                const $labelSpan = $rangeContainer.find(".ipt-4 .form-label span");

                if ($('#salary-all').is(':checked')) {
                    $labelSpan.text("Tất cả các mức lương");
                    $('#salary-range-section').slideUp();

                } else if ($('#salary-0-50').is(':checked')) {
                    let min = parseInt($('input.min').val()) || 0;
                    let max = parseInt($('input.max').val()) || 50000000;
                    $labelSpan.text(`${formatCurrencyVND(min)} VNĐ - ${formatCurrencyVND(max)} VNĐ`);
                    $('#salary-range-section').slideDown();

                } else if ($('#salary-above-50').is(':checked')) {
                    $labelSpan.text("Trên 50.000.000 VNĐ");
                    $('#salary-range-section').slideUp();
                }
            });



            $(document).on('input change', ".salary .range-input input", (e) => {
                console.log('Salary changed');

                let $rangeContainer = $(e.target).closest('.range-input');
                let $minInput = $rangeContainer.find(".min");
                let $maxInput = $rangeContainer.find(".max");

                let minSalary = parseInt($rangeContainer.find(".min").val());
                let maxSalary = parseInt($rangeContainer.find(".max").val());
                const STEP = 5000000; // 5 triệu
                const MIN_POSSIBLE = 0; // 0 đồng
                const MAX_POSSIBLE = 50000000; // 50 triệu


                // Phân biệt người dùng đang kéo min hay max
                if ($(e.target).hasClass('min')) {
                    // Ngăn không cho min vượt quá max - STEP
                    if (minSalary > maxSalary - STEP) {
                        minSalary = maxSalary - STEP;
                        $minInput.val(minSalary);
                    }
                } else {
                    // Ngăn không cho max nhỏ hơn min + STEP
                    if (maxSalary < minSalary + STEP) {
                        maxSalary = minSalary + STEP;
                        $maxInput.val(maxSalary);
                    }
                }

                // Clamp lại trong khoảng giới hạn (phòng khi người dùng dùng arrow key vượt)
                minSalary = Math.max(MIN_POSSIBLE, minSalary);
                maxSalary = Math.min(MAX_POSSIBLE, maxSalary);

                // Cập nhật lại giá trị cho tất cả slider
                $(".salary").each(function () {
                    let $salary = $(this);
                    let $range = $salary.find('.range-input');

                    $range.find(".min").val(minSalary);
                    $range.find(".max").val(maxSalary);

                    $range.find("input[name='salary_ranges[]'].min").val(minSalary);
                    $range.find("input[name='salary_ranges[]'].max").val(maxSalary);

                    FilterManager.updateSalaryDisplay(minSalary, maxSalary, $salary);
                });

                tempMinSalary = minSalary;
                tempMaxSalary = maxSalary;


                //this.filterState.salary = { min: minSalary, max: maxSalary };
                //console.log('Filter State:', this.filterState.salary);

            });

            $(".salary .ibtn").on("click", () => {
                let isRangeSelected = $('#salary-0-50').is(':checked');

                if ($('#salary-all').is(':checked')) {
                    this.filterState.salary = { type: "all", min: 0, max: 50000000 };
                } else if (isRangeSelected) {
                    this.filterState.salary = {
                        type: "range",
                        min: tempMinSalary,
                        max: tempMaxSalary
                    };
                } else if ($('#salary-above-50').is(':checked')) {
                    this.filterState.salary = {
                        type: "above50",
                        min: 50000000,
                        max: 100000000 // hoặc giá trị max bạn mong muốn
                    };
                }

                const $salaryDropdown = $(".salary");
                let displayText = "";

                if ($('#salary-all').is(':checked')) {
                    displayText = "Tất cả các mức lương";
                } else if (isRangeSelected) {
                    displayText = `${formatCurrencyVND(tempMinSalary)} VNĐ - ${formatCurrencyVND(tempMaxSalary)} VNĐ`;
                } else {
                    displayText = "Trên 50.000.000 VNĐ";
                }

                $salaryDropdown.find(".dropdown-model span:first").text(displayText);
                $salaryDropdown.find(".dropdown-model").addClass("selected");
                $salaryDropdown.find(".clearIcon").removeClass("d-none");
                $salaryDropdown.find(".chevronDownIcon").addClass("d-none");

                //this.fetchData();
                FilterManager.checkAnyFilterSelected();

                // Ẩn dropdown sau khi áp dụng
                $salaryDropdown.find(".dropdown-model").removeClass("show");
                $salaryDropdown.find(".dropdown-menu").removeClass("show");
            });
        }

        static updateSalaryDisplay(min, max, $container) {
            let minText = min === 0 ? "0" : formatCurrencyVND(min);
            let salaryText = `${minText} VNĐ - ${formatCurrencyVND(max)} VNĐ`;
            $container.find(".ipt-4 .form-label span").text(salaryText);
            //$container.find(".dropdown-model span:first").text(salaryText);

            let minPercent = (min / 50000000) * 100;
            let maxPercent = ((50000000 - max) / 50000000) * 100;

            $container.find(".range-selected").css({
                left: `${minPercent}%`,
                right: `${maxPercent}%`,
                "--min-salary-value": min,
                "--max-salary-value": max
            });
        }

        handleFilterClear() {
            $(".clearAll").click((e) => {
                e.preventDefault();
                $("input[type='checkbox']").prop('checked', false);
                $("input[name='salary_ranges[]'].min").val(0);
                $("input[name='salary_ranges[]'].max").val(5000000);
                FilterManager.updateSalaryDisplay(0, 5000000, $(".salary"));
                FilterManager.updateAllLabelsToDefault();
                this.filterState = {
                    jobLevels: [],
                    workingModels: [],
                    industries: [],
                    salary: { min: 0, max: 5000000, type: "all" },
                    companyTypes: []
                };
                FilterManager.checkAnyFilterSelected();
            });
        }

        handleModalToggle() {
            $(".button-filter").click(function () {
                let $filterJob = $("#filter-job");
                $("body").toggleClass("overflow-hidden", !$filterJob.hasClass("show"));
                $filterJob.toggleClass("show").css({ display: $filterJob.hasClass("show") ? "block" : "none", "background-color": "rgba(0,0,0, 0.3)" });
            });

            $(".btn-close").click(function () {
                $("#filter-job").removeClass("show").css("display", "none");
                $("body").removeClass("overflow-hidden");
            });
        }
        handleBtnSubmitModel = () => {
            $(".btnSubmitModel").click(function () {
                console.log(this.filterState)
                // $("form[action='/viec-lam-it']").submit(); 
            });
        }
        static updateFilterLabels(name) {
            let $dropdown = $(`input[name='${name}']`).closest('.dropdown');
            let $dropdownModel = $dropdown.find('.dropdown-model');
            let $labelText = $dropdownModel.find('.labelText');
            let $clearIcon = $dropdownModel.find('.clearIcon');
            let $chevronIcon = $dropdownModel.find('.chevronDownIcon');


            // Lấy tất cả checkbox được chọn
            let selectedCheckboxes = $dropdown.find(`input[name='${name}']:checked`);

            console.log(selectedCheckboxes)

            let selectedLabels = selectedCheckboxes.map(function () {
                return $(this).siblings('span').text().trim();
            }).get();

            // Kiểm tra số lượng checkbox được chọn
            if (selectedLabels.length > 0) {
                if (selectedLabels.length === 1) {
                    $labelText.text(selectedLabels[0]); // Hiển thị chỉ một lựa chọn
                } else {
                    $labelText.text(`${selectedLabels[0]} +${selectedLabels.length - 1}`); // Hiển thị "+1"
                }
                $dropdownModel.addClass('selected');
                $clearIcon.removeClass('d-none');
                $chevronIcon.addClass('d-none');
            } else {
                // Trả về mặc định nếu không chọn gì
                let defaultText = {
                    "job_level_names[]": "Cấp bậc",
                    "working_models[]": "Hình thức làm việc",
                    "company_industry_ids[]": "Lĩnh vực",
                    "company_types[]": "Loại công ty"
                };
                $labelText.text(defaultText[name] || "Chọn");
                $dropdownModel.removeClass('selected');
                $clearIcon.addClass('d-none');
                $chevronIcon.removeClass('d-none');
            }
        }

        static updateAllLabelsToDefault() {
            const defaultLabels = {
                "dropdown-salary": "Mức lương",
                "dropdown-industry": "Lĩnh vực",
                "dropdown-job-level": "Cấp bậc",
                "dropdown-working-model": "Hình thức làm việc",
                "dropdown-company-type": "Loại công ty"
            };
            $(".dropdown-model").each(function () {
                let $this = $(this);
                let id = $this.attr("id"); // lấy id, ví dụ: dropdown-salary
                let $labelText = $this.find(".labelText");

                // Gán text mặc định nếu có, nếu không thì để "Chọn"
                $labelText.text(defaultLabels[id] || "Chọn");

                // Reset trạng thái hiển thị
                $this.removeClass("selected");
                $this.find(".clearIcon").addClass("d-none");
                $this.find(".chevronDownIcon").removeClass("d-none");
            });
        }

        static checkAnyFilterSelected() {
            $(".clearInlineFilter").toggleClass('d-none', $(".dropdown-model.selected").length === 0);
        }

    }
    //new DropdownFilter('.inlineJobLevel', { defaultLabel: 'Cấp bậc' });
    //new DropdownFilter('.inlineWorking', { defaultLabel: 'Hình thức' });
    //new DropdownFilter('.inlineSalary', { defaultLabel: 'Mức lương' });
    //new DropdownFilter('.inlineIndustry', {
    //    defaultLabel: 'Ngành nghề',
    //    enableSearch: true, // Bật tìm kiếm
    //    searchInputSelector: '.industry-search',
    //    searchableItemSelector: '.items label.icheckbox',
    //    noResultSelector: '.noResult'
    //});
    //const savedFilters = {
    //    jobLevels: ['Junior', 'Senior'],
    //    workingModels: ['remote'],
    //    industries: [2, 5],
    //    salary: { type: 'range', min: 10000000, max: 20000000 },
    //    companyTypes: ['Outsourcing']
    //};

    //const modal = new FilterModal('#filterModal');

    //// Gán dữ liệu vào modal
    //modal.setFilterValues(savedFilters);

    //var filterManager = new FilterManager();

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

    // Đồng bộ ngược từ DropdownFilter → Modal
    dropdownJob.setOnChange((data) => modal.setFilterValues({ jobLevels: data.checkboxes }));
    dropdownWorking.setOnChange((data) => modal.setFilterValues({ workingModels: data.checkboxes }));
    dropdownIndustry.setOnChange((data) => modal.setFilterValues({ industries: data.checkboxes }));
    dropdownSalary.setOnChange((data) => modal.setFilterValues({ salary: data.salary }));
});
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
        });

        // ✅ Checkbox filter
        this.$checkboxes.on('change', () => this.updateCheckboxState());

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
            const min = this.$inputMin.val();
            const max = this.$inputMax.val();

            if (min || max) {
                let label = '';
                if (min && max) label = `${min} - ${max} triệu`;
                else if (min) label = `Trên ${min} triệu`;
                else if (max) label = `Dưới ${max} triệu`;

                this.setLabelAndClose(label);
            } else {
                const selected = this.$radioButtons.filter(':checked');
                if (selected.length > 0) {
                    const label = selected.siblings('label').text().trim();
                    this.setLabelAndClose(label);
                }
            }
        });
        if (this.options.enableSearch) {
            this.bindSearch();
        }

        this.$root.find('input').on('change', () => {
            console.log('🔍 Changed value in:', this.$root.attr('class'));
            console.log(this.getSelectedFilters());
        });
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
                    max: max ? Number(max) : null
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
                $cb.prop('checked', values.checkboxes.includes(val));
            });
        }

        if (values.radio) {
            this.$radioButtons.each(function () {
                const $rb = $(this);
                $rb.prop('checked', $rb.val() === values.radio);
            });
        }

        if (values.salary) {
            if (values.salary.min != null) this.$inputMin.val(values.salary.min);
            if (values.salary.max != null) this.$inputMax.val(values.salary.max);
        }

        // Cập nhật lại UI
        this.updateCheckboxState();
    }
    static initAll(selector, options = {}) {
        $(selector).each(function () {
            new DropdownFilter(this, options);
        });
    }
}

//class JobLevelFilter {
//    constructor(rootSelector, options = {}) {
//        this.$root = $(rootSelector);
//        this.$itag = this.$root.find('.itag');
//        this.$dropdownMenu = this.$root.find('.dropdown-menu');
//        this.$checkboxes = this.$root.find('input[type="checkbox"]');
//        this.$labelText = this.$root.find('.labelText');
//        this.$chevronIcon = this.$root.find('.chevronDownIcon');
//        this.$clearIcon = this.$root.find('.clearIcon');

//        // Lưu cấu hình, gán label mặc định
//        this.options = Object.assign({
//            defaultLabel: 'Lọc'
//        }, options);

//        this.bindEvents();
//        this.bindOutsideClick();
//    }

//    bindEvents() {
//        this.$checkboxes.on('change', () => this.updateState());
//        this.$clearIcon.on('click', (e) => {
//            e.stopPropagation();
//            this.clearAll();
//        });

//        this.$itag.on('click', (e) => {
//            e.stopPropagation();

//            // Nếu có dropdown khác đang mở → đóng nó
//            if (JobLevelFilter.activeInstance && JobLevelFilter.activeInstance !== this) {
//                JobLevelFilter.activeInstance.closeDropdown();
//            }

//            // Toggle dropdown hiện tại
//            const isOpen = this.$dropdownMenu.hasClass('show');
//            if (!isOpen) {
//                this.openDropdown();
//                JobLevelFilter.activeInstance = this;
//            } else {
//                this.closeDropdown();
//                JobLevelFilter.activeInstance = null;
//            }
//        });
//    }

//    bindOutsideClick() {
//        $(document).on('click', (e) => {
//            const isClickInside = $(e.target).closest(this.$root).length > 0;

//            // Nếu click ra ngoài dropdown
//            if (!isClickInside) {
//                this.closeDropdown();

//                // Nếu đây là dropdown đang mở → clear trạng thái active
//                if (JobLevelFilter.activeInstance === this) {
//                    JobLevelFilter.activeInstance = null;
//                }
//            }
//        });
//    }

//    updateState() {
//        const checkedBoxes = this.$checkboxes.filter(':checked');

//        const values = checkedBoxes.map(function () {
//            return $(this).siblings('span').text().trim();
//        }).get();

//        const count = values.length;

//        if (count > 0) {
//            this.$itag.addClass('show selected');
//            this.$dropdownMenu.addClass('show');
//            this.$chevronIcon.addClass('d-none');
//            this.$clearIcon.removeClass('d-none');

//            this.$labelText.empty();
//            this.$labelText.append(`<span class="label-badge">${values[0]}</span>`);
//            if (count > 1) {
//                this.$labelText.append(`<span class="label-badge">+${count - 1}</span>`);
//            }
//        } else {
//            this.resetUI();
//        }
//    }

//    openDropdown() {
//        this.$itag.addClass('show');
//        this.$dropdownMenu.addClass('show');
//    }

//    closeDropdownIfNotSelected() {
//        if (!this.$itag.hasClass('selected')) {
//            this.$itag.removeClass('show');
//            this.$dropdownMenu.removeClass('show');
//        }
//    }
//    closeDropdown() {
//        this.$itag.removeClass('show');
//        this.$dropdownMenu.removeClass('show');
//    }
//    clearAll() {
//        this.$checkboxes.prop('checked', false);
//        this.resetUI();
//    }

//    resetUI() {
//        this.$itag.removeClass('show selected');
//        this.$dropdownMenu.removeClass('show');
//        this.$chevronIcon.removeClass('d-none');
//        this.$clearIcon.addClass('d-none');
//        this.$labelText.text(this.options.defaultLabel); // 👈 dùng defaultLabel
//    }
//}



//class DropdownFilter {
//    constructor(rootSelector, options = {}) {
//        this.$root = $(rootSelector);
//        this.$itag = this.$root.find('.itag');
//        this.$dropdownMenu = this.$root.find('.dropdown-menu');
//        this.$labelText = this.$root.find('.labelText');
//        this.$chevronIcon = this.$root.find('.chevronDownIcon');
//        this.$clearIcon = this.$root.find('.clearIcon');
//        this.$radioButtons = this.$root.find('input[type="radio"][name]');
//        this.$applyButton = this.$root.find('.ibtn');
//        this.$inputMin = this.$root.find('#salary-min');
//        this.$inputMax = this.$root.find('#salary-max');

//        this.options = Object.assign({ defaultLabel: 'Lọc' }, options);

//        this.bindEvents();
//        this.bindOutsideClick();
//    }

//    bindEvents() {
//        this.$itag.on('click', (e) => {
//            e.stopPropagation();

//            if (DropdownFilter.activeInstance && DropdownFilter.activeInstance !== this) {
//                DropdownFilter.activeInstance.closeDropdown();
//            }

//            const isOpen = this.$dropdownMenu.hasClass('show');
//            if (!isOpen) {
//                this.openDropdown();
//                DropdownFilter.activeInstance = this;
//            } else {
//                this.closeDropdown();
//                DropdownFilter.activeInstance = null;
//            }
//        });

//        this.$clearIcon.on('click', (e) => {
//            e.stopPropagation();
//            this.clearAll();
//        });

//        // Radio change → cập nhật input nếu có (chỉ dùng cho salary type)
//        this.$radioButtons.on('change', (e) => {
//            const label = $(e.target).siblings('label').text().trim();
//            if (this.$inputMin.length && this.$inputMax.length) {
//                const match = label.match(/(\d+)\s*-\s*(\d+)/);
//                if (match) {
//                    this.$inputMin.val(match[1]);
//                    this.$inputMax.val(match[2]);
//                } else if (label.includes('Dưới')) {
//                    const max = label.match(/\d+/);
//                    this.$inputMin.val('');
//                    this.$inputMax.val(max ? max[0] : '');
//                } else if (label.includes('Trên')) {
//                    const min = label.match(/\d+/);
//                    this.$inputMin.val(min ? min[0] : '');
//                    this.$inputMax.val('');
//                } else {
//                    this.$inputMin.val('');
//                    this.$inputMax.val('');
//                }
//            }
//        });

//        this.$applyButton.on('click', () => {
//            const min = this.$inputMin.val();
//            const max = this.$inputMax.val();

//            if (min || max) {
//                let label = '';
//                if (min && max) label = `${min} - ${max} triệu`;
//                else if (min) label = `Trên ${min} triệu`;
//                else if (max) label = `Dưới ${max} triệu`;

//                this.setLabelAndClose(label);
//            } else {
//                const selected = this.$radioButtons.filter(':checked');
//                if (selected.length > 0) {
//                    const label = selected.siblings('label').text().trim();
//                    this.setLabelAndClose(label);
//                }
//            }
//        });
//    }

//    bindOutsideClick() {
//        $(document).on('click', (e) => {
//            const isClickInside = $(e.target).closest(this.$root).length > 0;
//            if (!isClickInside) {
//                this.closeDropdown();
//                if (DropdownFilter.activeInstance === this) {
//                    DropdownFilter.activeInstance = null;
//                }
//            }
//        });
//    }

//    setLabelAndClose(label) {
//        this.$labelText.empty().append(`<span class="label-badge">${label}</span>`);
//        this.$itag.addClass('selected');
//        this.closeDropdown();
//        this.$clearIcon.removeClass('d-none');
//        this.$chevronIcon.addClass('d-none');
//    }

//    clearAll() {
//        this.$radioButtons.prop('checked', false);
//        this.$inputMin.val && this.$inputMin.val('');
//        this.$inputMax.val && this.$inputMax.val('');
//        this.resetUI();
//    }

//    resetUI() {
//        this.$itag.removeClass('selected show');
//        this.$dropdownMenu.removeClass('show');
//        this.$labelText.text(this.options.defaultLabel);
//        this.$clearIcon.addClass('d-none');
//        this.$chevronIcon.removeClass('d-none');
//    }

//    openDropdown() {
//        this.$itag.addClass('show');
//        this.$dropdownMenu.addClass('show');
//    }

//    closeDropdown() {
//        this.$itag.removeClass('show');
//        this.$dropdownMenu.removeClass('show');
//    }

//    static initAll(selector, options = {}) {
//        $(selector).each(function () {
//            new DropdownFilter(this, options);
//        });
//    }
//}


    class FilterModal {
        constructor(modalSelector, linkedDropdowns = {}) {
            this.$modal = $(modalSelector);
            this.$form = this.$modal.find('form');
            this.$inputs = this.$modal.find('input');
            this.$searchInput = this.$modal.find('.modalIndustry input[type="text"]');
            this.$searchItems = this.$modal.find('.modalIndustry .items label.icheckbox');
            this.$noResult = this.$modal.find('.modalIndustry .noResult');
            this.$resetBtn = this.$modal.find('a[href="/viec-lam-it"]');
            this.linkedDropdowns = linkedDropdowns; 
            console.log(this.$noResult)
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

            this.$modal.find('#salary-min, #salary-max').on('input', () => {
                this.clearSalarySelections();
            });
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
                salary: {},
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
                data.industries.push(Number($(el).val()));
            });

            // companyTypes[]
            this.$modal.find('input[name="company_types[]"]:checked').each((_, el) => {
                data.companyTypes.push($(el).val());
            });

            // salary range
            const min = parseInt(this.$modal.find('#salary-min').val(), 10);
            const max = parseInt(this.$modal.find('#salary-max').val(), 10);

            if (!isNaN(min) || !isNaN(max)) {
                data.salary.type = 'range';
                data.salary.min = isNaN(min) ? 0 : min;
                data.salary.max = isNaN(max) ? 0 : max;
            } else {
                data.salary.type = 'none';
            }

            return data;
        }

        bindSubmitAndReset() {
            // "Áp dụng" – lấy dữ liệu
            //this.$modal.find('.btnSubmitModel').on('click', (e) => {
            //    e.preventDefault();
            //    const filters = this.getSelectedFilters();
            //    console.log('✅ Filters selected:', filters);

            //    // TODO: gửi filters đi API hoặc xử lý tiếp
            //});
            this.$modal.find('.btnSubmitModel').on('click', (e) => {
                e.preventDefault();
                const filters = this.getSelectedFilters();

                // ✅ Gửi lại data về các DropdownFilter tương ứng
                for (const key in this.linkedDropdowns) {
                    const dropdown = this.linkedDropdowns[key];
                    if (dropdown && typeof dropdown.setFilterValues === 'function') {
                        const sectionValue = filters[key];

                        if (sectionValue) {
                            // chuyển về đúng định dạng
                            const dropdownData = Array.isArray(sectionValue)
                                ? { checkboxes: sectionValue }
                                : sectionValue.type === 'range'
                                    ? { salary: sectionValue }
                                    : {};

                            dropdown.setFilterValues(dropdownData);
                        }
                    }
                }

                console.log('✅ Đồng bộ ngược về DropdownFilter:', filters);
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
            // 1. Check job level
            filters.jobLevels?.forEach(val => {
                this.$modal.find(`input[name="job_level_names[]"][value="${val}"]`).prop('checked', true);
            });

            // 2. Working model
            filters.workingModels?.forEach(val => {
                this.$modal.find(`input[name="working_models[]"][value="${val}"]`).prop('checked', true);
            });

            // 3. Industries
            filters.industries?.forEach(val => {
                this.$modal.find(`input[name="company_industry_ids[]"][value="${val}"]`).prop('checked', true);
            });

            // 4. Company types
            filters.companyTypes?.forEach(val => {
                this.$modal.find(`input[name="company_types[]"][value="${val}"]`).prop('checked', true);
            });

            // 5. Salary
            if (filters.salary?.type === 'range') {
                if (filters.salary.min) this.$modal.find('#salary-min').val(filters.salary.min);
                if (filters.salary.max) this.$modal.find('#salary-max').val(filters.salary.max);
            }

            // Cập nhật lại UI sau khi set value
            this.updateCheckedUI();
        }
    }
