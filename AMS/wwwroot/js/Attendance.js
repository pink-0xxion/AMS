$(document).ready(() => {
    //alert("Hello!");
    GetEmployees();

    // Show default table when page loads
    setTimeout(() => {
        fetchAttendance();
    }, 200); // Delay to ensure GetEmployees has populated the dropdown

    // Safe as it only runs when form already rendered, placing of <script> doesn't matter
    $('#attendanceForm').on('submit', function (e) {
        e.preventDefault(); // prevent form default postback

        const employeeId = $('#employee').val();
        const month = $('select[name="month"]').val();
        const year = $('select[name="year"]').val();

        if (!employeeId || !month || !year) {
            alert("Please select employee, month, and year.");
            return;
        }

        fetchAttendance();
    });
})


// Write outside, if DOM or Html form element already rendered, place it in bottom near </body>
//$('#attendanceForm').on('submit', function (e) {
//    e.preventDefault(); // prevent form default postback

//    const employeeId = $('#employee').val();
//    const month = $('select[name="month"]').val();
//    const year = $('select[name="year"]').val();

//    if (!employeeId || !month || !year) {
//        alert("Please select employee, month, and year.");
//        return;
//    }

//    fetchAttendance();
//});

function GetEmployees() {
    console.log("GetEmployees is running");

    $.ajax({
        url: '/Attendance/GetEmployees',
        method: 'GET',
        success: function (result) {
            // Add "All Employees" option first
            $('#employee').append('<option value="0">All Employees</option>');

            // Then loop through actual employees
            $.each(result, function (i, data) {
                $('#employee').append('<option value="' + data.id + '">' + data.name + '</option>');
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching employees:", error);
        }
    });

}

function fetchAttendance() {
    console.log("fetchAttendance is called");


    //const employeeId = $('#employee').val();
    //const month = $('select[name="month"]').val();
    //const year = $('select[name="year"]').val();

    // Get current month and year
    const today = new Date();
    const currentMonth = today.getMonth() + 1; // getMonth() returns 0-11
    const currentYear = today.getFullYear();

    // default fallback
    const employeeId = $('#employee').val() ? $('#employee').val() : 0;
    const month = $('select[name="month"]').val() ? $('select[name="month"]').val() : currentMonth;
    const year = $('select[name="year"]').val() ? $('select[name="year"]').val() : currentYear;

    console.log("employeeId: ", employeeId);
    console.log("month: ", month);
    console.log("year: ", year);


    //if (!employeeId || !month || !year) {
    //    alert("Please select employee, month, and year.");
    //    return;
    //}

    $.ajax({
        url: '/Attendance/GetEmployeeAttendance',
        method: 'GET',
        data: {
            employee: employeeId,
            month: month,
            year: year
        },
        success: function (data) {
            console.log("Attendance Data:", data);

            const tableHead = $('#attendanceHead');
            const tableBody = $('#attendanceBody');
            const tableContainer = $('#attendanceTableContainer'); // wrap your table in a div with this ID
            const messageContainer = $('#noDataMessage'); // a div for showing the message
            const instructions = $('#instructions'); // holds the instructions

            tableHead.empty();
            tableBody.empty();

            //tableContainer.hide();

            if (!data || data.length === 0) {
                tableContainer.hide();               // hide table
                instructions.addClass("d-none");
                messageContainer.text("No attendance data found. Please check the employee name, month, or year.").show();
                return;
            }

            // Hide message and show table
            messageContainer.hide();
            tableContainer.show();
            instructions.removeClass("d-none");
            instructions.addClass("d-flex");

            const daysInMonth = new Date(year, month, 0).getDate();

            // Build the header row
            let headerRow1 = `<tr><th rowspan="2">Name</th>`;
            let headerRow2 = `<tr>`;
            for (let d = 1; d <= daysInMonth; d++) {
                const dateObj = new Date(year, month - 1, d);
                headerRow1 += `<th>${d}</th>`;
                headerRow2 += `<th>${dateObj.toLocaleDateString('en-us', { weekday: 'short' })}</th>`;
            }
            headerRow1 += `</tr>`;
            headerRow2 += `</tr>`;
            tableHead.append(headerRow1);
            tableHead.append(headerRow2);

            // Group attendance data by employee
            const grouped = {};
            data.forEach(item => {
                const date = new Date(item.attendanceDate).getDate();
                const empId = item.employeeID;
                if (!grouped[empId]) {
                    grouped[empId] = {
                        name: item.employeeName || `Employee ${empId}`,
                        attendance: {}
                    };
                }
                grouped[empId].attendance[date] = item.status;
            });

            // Build rows
            for (const empId in grouped) {
                const employee = grouped[empId];
                //let row = `<tr><td>${employee.name}</td>`;
                let row = `<tr><td><a href="/Admin/Dashboard/EmployeeDetails/${empId}">${employee.name}</a></td>`;
                //let employeeUrl = employeeDetailsBaseUrl.replace('__ID__', empId);
                //let row = `<tr><td><a href="${employeeUrl}">${employee.name}</a></td>`;
                for (let d = 1; d <= daysInMonth; d++) {
                    const status = employee.attendance[d];
                    if (status === "Present") {
                        row += "<td>✔️</td>";
                    } else if (status === "Absent") {
                        row += "<td>❌</td>";
                    } else if (status === "Leave") {
                        row += "<td>⏸️</td>"; // Yellow dot for Leave
                    } else if (status === "Half Day") {
                        row += "<td>🌓</td>"; // Pause icon for Half Day
                    } else {
                        row += "<td></td>"; // No data
                    }
                }

                row += "</tr>";
                tableBody.append(row);
            }
        },
        error: function (err) {
            console.error("Error fetching attendance:", err);
        }
    });
}

