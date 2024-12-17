-- Employee Management System SQL Implementation

-- 1. Database Schema Creation

-- Create Departments table
CREATE TABLE Departments (
    DepartmentID INT PRIMARY KEY,
    DepartmentName VARCHAR(100) NOT NULL
);

-- Create Employees table
CREATE TABLE Employees (
    EmployeeID INT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    DepartmentID INT NOT NULL,
    HireDate DATE NOT NULL,
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);

-- Create Salaries table
CREATE TABLE Salaries (
    EmployeeID INT PRIMARY KEY,
    BaseSalary DECIMAL(10, 2) NOT NULL,
    Bonus DECIMAL(10, 2) DEFAULT 0,
    Deductions DECIMAL(10, 2) DEFAULT 0,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

-- Bonus: Create SalaryHistory table to log salary updates
CREATE TABLE SalaryHistory (
    HistoryID INT AUTO_INCREMENT PRIMARY KEY,
    EmployeeID INT NOT NULL,
    ChangeDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    OldBaseSalary DECIMAL(10, 2),
    NewBaseSalary DECIMAL(10, 2),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

-- 2. Queries

-- Query 1: List all employees along with their department names
SELECT 
    e.EmployeeID, 
    e.Name, 
    d.DepartmentName
FROM 
    Employees e
JOIN 
    Departments d ON e.DepartmentID = d.DepartmentID;

-- Query 2: Calculate net salary for each employee
SELECT 
    e.EmployeeID, 
    e.Name, 
    s.BaseSalary, 
    s.Bonus, 
    s.Deductions,
    (s.BaseSalary + s.Bonus - s.Deductions) AS NetSalary
FROM 
    Employees e
JOIN 
    Salaries s ON e.EmployeeID = s.EmployeeID;

-- Query 3: Identify the department with the highest average salary
SELECT 
    d.DepartmentName, 
    AVG(s.BaseSalary + s.Bonus - s.Deductions) AS AverageSalary
FROM 
    Employees e
JOIN 
    Salaries s ON e.EmployeeID = s.EmployeeID
JOIN 
    Departments d ON e.DepartmentID = d.DepartmentID
GROUP BY 
    d.DepartmentName
ORDER BY 
    AverageSalary DESC
LIMIT 1;

-- 3. Stored Procedures

-- Procedure to add a new employee
DELIMITER //
CREATE PROCEDURE AddEmployee (
    IN empName VARCHAR(100),
    IN deptID INT,
    IN hireDate DATE
)
BEGIN
    IF EXISTS (SELECT 1 FROM Departments WHERE DepartmentID = deptID) THEN
        INSERT INTO Employees (Name, DepartmentID, HireDate)
        VALUES (empName, deptID, hireDate);
    ELSE
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Invalid DepartmentID';
    END IF;
END //
DELIMITER ;

-- Procedure to update salary and log changes
DELIMITER //
CREATE PROCEDURE UpdateSalary (
    IN empID INT,
    IN newBaseSalary DECIMAL(10, 2)
)
BEGIN
    DECLARE oldBaseSalary DECIMAL(10, 2);

    SELECT BaseSalary INTO oldBaseSalary
    FROM Salaries
    WHERE EmployeeID = empID;

    IF oldBaseSalary IS NOT NULL THEN
        UPDATE Salaries
        SET BaseSalary = newBaseSalary
        WHERE EmployeeID = empID;

        INSERT INTO SalaryHistory (EmployeeID, OldBaseSalary, NewBaseSalary)
        VALUES (empID, oldBaseSalary, newBaseSalary);
    ELSE
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Employee not found';
    END IF;
END //
DELIMITER ;

-- Procedure to calculate total payroll
DELIMITER //
CREATE PROCEDURE CalculatePayroll (
    IN deptID INT,
    OUT totalPayroll DECIMAL(10, 2)
)
BEGIN
    SELECT SUM(BaseSalary + Bonus - Deductions) INTO totalPayroll
    FROM Salaries s
    JOIN Employees e ON s.EmployeeID = e.EmployeeID
    WHERE e.DepartmentID = deptID;
END //
DELIMITER ;

-- 4. Views

-- View to show detailed employee salary report
CREATE VIEW EmployeeSalaryView AS
SELECT 
    e.EmployeeID, 
    e.Name, 
    d.DepartmentName, 
    s.BaseSalary, 
    s.Bonus, 
    s.Deductions, 
    (s.BaseSalary + s.Bonus - s.Deductions) AS NetSalary
FROM 
    Employees e
JOIN 
    Salaries s ON e.EmployeeID = s.EmployeeID
JOIN 
    Departments d ON e.DepartmentID = d.DepartmentID;

-- View to list high earners
CREATE VIEW HighEarnerView AS
SELECT 
    e.EmployeeID, 
    e.Name, 
    (s.BaseSalary + s.Bonus - s.Deductions) AS NetSalary
FROM 
    Employees e
JOIN 
    Salaries s ON e.EmployeeID = s.EmployeeID
WHERE 
    (s.BaseSalary + s.Bonus - s.Deductions) > 50000; -- Threshold value

-- 5. Optimization Strategies

-- Indexing for faster queries
CREATE INDEX idx_department_id ON Employees (DepartmentID);
CREATE INDEX idx_employee_id ON Salaries (EmployeeID);

-- Example Output:
-- 1. List of employees with departments
-- 2. Detailed salary report using EmployeeSalaryView
-- 3. High earners using HighEarnerView
-- 4. Total payroll calculated using CalculatePayroll stored procedure
