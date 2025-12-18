CREATE TABLE IF NOT EXISTS user (
    userId INT AUTO_INCREMENT PRIMARY KEY,
    userName VARCHAR(50),
    password VARCHAR(50),
    active TINYINT(1),
    createDate DATETIME,
    createdBy VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS country (
    countryId INT AUTO_INCREMENT PRIMARY KEY,
    country VARCHAR(50),
    createDate DATETIME,
    createdBy VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS city (
    cityId INT AUTO_INCREMENT PRIMARY KEY,
    city VARCHAR(50),
    countryId INT,
    createDate DATETIME,
    createdBy VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS address (
    addressId INT AUTO_INCREMENT PRIMARY KEY,
    address VARCHAR(100),
    address2 VARCHAR(100),
    cityId INT,
    postalCode VARCHAR(20),
    phone VARCHAR(30),
    createDate DATETIME,
    createdBy VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS customer (
    customerId INT AUTO_INCREMENT PRIMARY KEY,
    customerName VARCHAR(100),
    addressId INT,
    active TINYINT(1),
    createDate DATETIME,
    createdBy VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS appointment (
    appointmentId INT AUTO_INCREMENT PRIMARY KEY,
    customerId INT,
    userId INT,
    title VARCHAR(100),
    description VARCHAR(255),
    location VARCHAR(100),
    contact VARCHAR(50),
    type VARCHAR(50),
    url VARCHAR(255),
    start DATETIME,
    end DATETIME,
    createDate DATETIME,
    createdBy VARCHAR(50),
    lastUpdate DATETIME
);

INSERT INTO user (userName, password, active, createDate, createdBy) VALUES ('test', 'test', 1, NOW(), 'system');

INSERT INTO country (country, createDate, createdBy) VALUES ('United States', NOW(), 'system');
INSERT INTO city (city, countryId, createDate, createdBy) VALUES ('Phoenix', 1, NOW(), 'system');
INSERT INTO address (address, address2, cityId, postalCode, phone, createDate, createdBy) VALUES ('100 Main St', '', 1, '85001', '602-555-1212', NOW(), 'system');
INSERT INTO customer (customerName, addressId, active, createDate, createdBy) VALUES ('Acme Corp', 1, 1, NOW(), 'system');

INSERT INTO appointment (customerId, userId, title, description, type, start, end, createDate, createdBy)
VALUES (1, 1, 'Consultation', 'Initial meeting', 'Consult', '2025-11-10 15:00:00', '2025-11-10 15:30:00', NOW(), 'system');
