ko.bindingHandlers.dataTablesForEach = {
    page: 0,
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var options = ko.unwrap(valueAccessor());
        ko.unwrap(options.data);
        if (options.dataTableOptions.paging) {
            valueAccessor().data.subscribe(function (changes) {
                var table = $(element).closest('table').DataTable();
                ko.bindingHandlers.dataTablesForEach.page = table.page();
                table.destroy();
            }, null, 'arrayChange');
        }
        var nodes = Array.prototype.slice.call(element.childNodes, 0);
        ko.utils.arrayForEach(nodes, function (node) {
            if (node && node.nodeType !== 1) {
                node.parentNode.removeChild(node);
            }
        });
        return ko.bindingHandlers.foreach.init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        $(element).closest('table').DataTable().destroy();
        var options = ko.unwrap(valueAccessor()),
            key = 'DataTablesForEach_Initialized';
        ko.unwrap(options.data);
        var table;
        if (!options.dataTableOptions.paging) {
            table = $(element).closest('table').DataTable();
            table.destroy();
        }
        ko.bindingHandlers.foreach.update(element, valueAccessor, allBindings, viewModel, bindingContext);
        table = $(element).closest('table').DataTable(options.dataTableOptions);
        if (options.dataTableOptions.paging) {
            if (table.page.info().pages - ko.bindingHandlers.dataTablesForEach.page == 0)
                table.page(--ko.bindingHandlers.dataTablesForEach.page).draw(false);
            else
                table.page(ko.bindingHandlers.dataTablesForEach.page).draw(false);
        }
        if (!ko.utils.domData.get(element, key) && (options.data || options.length))
            ko.utils.domData.set(element, key, true);

        tableA(table, $(element).closest('table'));
        return { controlsDescendantBindings: true };
    }
};
var custom_datatables_rows_selected = [];
function updateDataTableSelectAllCtrl(table) {
    var $table = table.table().node();
    var $chkbox_all = $('tbody input[type="checkbox"]', $table);
    var $chkbox_checked = $('tbody input[type="checkbox"]:checked', $table);
    var chkbox_select_all = $('thead input[name="select_all"]', $table).get(0);

    // If none of the checkboxes are checked
    if ($chkbox_checked.length === 0) {
        chkbox_select_all.checked = false;
        if ('indeterminate' in chkbox_select_all) {
            chkbox_select_all.indeterminate = false;
        }

        // If all of the checkboxes are checked
    } else if ($chkbox_checked.length === $chkbox_all.length) {
        chkbox_select_all.checked = true;
        if ('indeterminate' in chkbox_select_all) {
            chkbox_select_all.indeterminate = false;
        }
        $("#custom_datatables_submit_button").css("display", "block");

        // If some of the checkboxes are checked
    } else {
        chkbox_select_all.checked = true;
        if ('indeterminate' in chkbox_select_all) {
            chkbox_select_all.indeterminate = true;
        }
    }
}
function updateDataTableSubmitButton() {
    if (custom_datatables_rows_selected.length > 0) {
        $("#custom_datatables_submit_button").css("display", "block");
    }
    else {
        $("#custom_datatables_submit_button").css("display", "none");
    }
}
function tableA(table, element) {
    $('#' + element[0].id + ' tbody').on('click', 'input[type="checkbox"]', function (e) {
        var $row = $(this).closest('tr');

        // Get row data
        var data = table.row($row).data();

        // Get row ID
        var rowId = $row.children().siblings(":first").text();

        // Determine whether row ID is in the list of selected row IDs 
        var index = $.inArray(rowId, custom_datatables_rows_selected);

        // If checkbox is checked and row ID is not in list of selected row IDs
        if (this.checked && index === -1) {
            custom_datatables_rows_selected.push(rowId);

            // Otherwise, if checkbox is not checked and row ID is in list of selected row IDs
        } else if (!this.checked && index !== -1) {
            custom_datatables_rows_selected.splice(index, 1);
        }

        if (this.checked) {
            $row.addClass('selected');
        } else {
            $row.removeClass('selected');
        }

        // Update state of "Select all" control
        updateDataTableSelectAllCtrl(table);
        updateDataTableSubmitButton();

        // Prevent click event from propagating to parent
        e.stopPropagation();
    });

    // Handle click on table cells with checkboxes
    //$('#' + element[0].id + '').on('click', 'tbody td, thead th:first-child', function (e) {
    //    $(this).parent().find('input[type="checkbox"]').trigger('click');
    //});

    // Handle click on "Select all" control
    $('thead input[name="select_all"]', table.table().container()).on('click', function (e) {
        if (this.checked) {
            $('#' + element[0].id + ' tbody input[type="checkbox"]:not(:checked)').trigger('click');
        } else {
            $('#' + element[0].id + ' tbody input[type="checkbox"]:checked').trigger('click');
        }

        // Prevent click event from propagating to parent
        e.stopPropagation();
    });

    // Handle table draw event
    table.on('draw', function () {
        // Update state of "Select all" control
        updateDataTableSelectAllCtrl(table);
        updateDataTableSubmitButton();
    });
}
