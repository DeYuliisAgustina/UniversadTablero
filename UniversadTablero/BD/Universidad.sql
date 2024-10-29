DROP DATABASE Universidad

USE master

CREATE DATABASE Universidad
GO

USE Universidad
GO

CREATE TABLE Usuario (
    UsuarioId INT PRIMARY KEY IDENTITY(1,1), 
	Correo NVARCHAR (150) NULL,
    Clave NVARCHAR(160) NULL
);

CREATE TABLE Profesores (
    ProfesorID INT PRIMARY KEY IDENTITY(1,1),
    NombreyApellido NVARCHAR(150) NOT NULL,
    DNI NVARCHAR(20) NOT NULL,
    FechaNacimiento DATE,
    Correo NVARCHAR(100),
    Telefono NVARCHAR(20)
)

CREATE TABLE Materias (
    MateriaID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR (257),
    ProfesorID INT,
    FOREIGN KEY (ProfesorID) REFERENCES Profesores(ProfesorID)
);

CREATE TABLE Alumnos (
    AlumnoID INT PRIMARY KEY IDENTITY(1,1),
    NombreyApellido NVARCHAR(150) NOT NULL,
    DNI NVARCHAR(20) NOT NULL,
    FechaNacimiento DATE,
    Domicilio NVARCHAR(255),
    Correo NVARCHAR(100),
    Telefono NVARCHAR(20),
);

CREATE TABLE Notas (
    NotaID INT PRIMARY KEY IDENTITY(1,1),
    AlumnoID INT,
    MateriaID INT,
    Valor DECIMAL(5, 2) NOT NULL,
    Fecha DATE NOT NULL,
    FOREIGN KEY (AlumnoID) REFERENCES Alumnos(AlumnoID),
    FOREIGN KEY (MateriaID) REFERENCES Materias(MateriaID)
);

CREATE TABLE AlumnoMaterias (
    AlumnoID INT,
    MateriaID INT,
	Estado NVARCHAR (150) NOT NULL,
    PRIMARY KEY (AlumnoID, MateriaID),
    FOREIGN KEY (AlumnoID) REFERENCES Alumnos(AlumnoID),
    FOREIGN KEY (MateriaID) REFERENCES Materias(MateriaID)
);

--PROCEDURES LOGIN
CREATE PROC Sp_RegistrarUsuario(
@Correo NVARCHAR(150),
@Clave NVARCHAR (160),
@Registrado BIT OUTPUT,
@Mensaje NVARCHAR (100) OUTPUT
)		
AS
BEGIN
    IF (NOT EXISTS(SELECT * FROM Usuario WHERE Correo = @Correo))
    BEGIN
        INSERT INTO Usuario (Correo, Clave) VALUES (@Correo, @Clave);
        SET @Registrado = 1;
        SET @Mensaje = 'Usuario registrado';
    END
    ELSE
    BEGIN
        SET @Registrado = 0;
        SET @Mensaje = 'Correo ya existe';
    END
END

--Validar si el usuario existe en la tabla
CREATE PROC Sp_ValidarUsuario(
@Correo NVARCHAR(150),
@Clave NVARCHAR (160)
)
AS
BEGIN
	IF(exists(SELECT * FROM Usuario WHERE Correo = @Correo and Clave=@Clave))
		SELECT UsuarioID FROM Usuario WHERE Correo=@Correo and Clave=@Clave
	ELSE
		SELECT '0'  --si no se encontro retorna cero
END

--Testeo (ejecuto una vez= usuario registrado, ejecuto 2da vez= correo ya existe)
DECLARE @registrado BIT, @mensaje VARCHAR(100)
EXEC Sp_RegistrarUsuario 'admin@gmail.com', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', @registrado OUTPUT,@mensaje OUTPUT
SELECT @registrado --su valor es 0.
SELECT @mensaje --su valor es correo ya existe.

EXEC Sp_ValidarUsuario 'admin@gmail.com', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9'

SELECT * FROM Usuario
--------------FIN TESTEO SP --------------

-- 1. Inserto Profesores
INSERT INTO Profesores (NombreyApellido, DNI, FechaNacimiento, Correo, Telefono)
VALUES 
    ('Alejandro Medina', '28567432', '1983-07-12', 'amedina@universidad.edu', '1123678901'),
    ('Patricia Rojas', '27890543', '1981-09-28', 'projas@universidad.edu', '1134789012'),
    ('Gabriel Castro', '29345678', '1984-03-15', 'gcastro@universidad.edu', '1145890123'),
    ('Valeria Mendoza', '26789054', '1980-11-05', 'vmendoza@universidad.edu', '1156901234'),
    ('Diego Peralta', '30234567', '1985-04-20', 'dperalta@universidad.edu', '1167012345');

-- 2. Inserto Materias
INSERT INTO Materias (Nombre, Descripcion, ProfesorID)
VALUES 
    ('Desarrollo Web', 'Frontend y Backend con tecnologías actuales', 1),
    ('Base de Datos', 'Diseño y administración de bases de datos', 2),
    ('Inteligencia Artificial', 'Machine Learning y Deep Learning', 3),
    ('Ciberseguridad', 'Seguridad informática y ethical hacking', 4),
    ('Comunicaciones II', 'Fundamentos de redes y protocolos de comunicación', 5);

-- 3. Inserto Alumnos
INSERT INTO Alumnos (NombreyApellido, DNI, FechaNacimiento, Domicilio, Correo, Telefono)
VALUES 
    ('Tomás Herrera', '42789123', '2003-05-18', 'Av. Libertador 2345', 'therrera@universidad.edu', '1178123456'),
    ('Valentina Silva', '42890234', '2004-02-15', 'Belgrano 1678', 'vsilva@universidad.edu', '1189234567'),
    ('Lucas Morales', '42901345', '2003-09-22', 'San Martín 890', 'lmorales@universidad.edu', '1190345678'),
    ('Sofía Lagos', '43012456', '2004-07-30', 'Corrientes 3456', 'slagos@universidad.edu', '1101456789'),
    ('Mateo Paz', '43123567', '2003-11-12', 'Santa Fe 2345', 'mpaz@universidad.edu', '1112567890');

-- 4. Inserto AlumnoMaterias
INSERT INTO AlumnoMaterias (AlumnoID, MateriaID,Estado)
VALUES 
    (1, 1, 'Aprobado' ), -- Tomás - Desarrollo Web
    (1, 2, 'Desaprobado'), -- Tomás - Base de Datos
    (2, 2, 'Regular' ), -- Valentina - Base de Datos
    (2, 3,'Aprobado' ), -- Valentina - Inteligencia Artificial
    (3, 3,'Aprobado' ), -- Lucas - Inteligencia Artificial
    (3, 4, 'Desaprobado'), -- Lucas - Ciberseguridad
    (4, 4, 'Regular' ), -- Sofía - Ciberseguridad
    (4, 5, 'Aprobado' ), -- Sofía - Comunicaciones II
    (5, 1,'Desaprobado'), -- Mateo - Desarrollo Web
    (5, 5,'Aprobado'); -- Mateo - Comunicaciones II

-- 5. Inserto Notas
INSERT INTO Notas (AlumnoID, MateriaID, Valor, Fecha)
VALUES 
    (1, 1, 8.75, '2024-03-15'), -- Tomás - Desarrollo Web
    (1, 2, 2.50, '2024-03-18'), -- Tomás - Base de Datos
    (2, 2, 4.25, '2024-03-18'), -- Valentina - Base de Datos
    (2, 3, 8.00, '2024-03-20'), -- Valentina - Inteligencia Artificial
    (3, 3, 7.75, '2024-03-20'), -- Lucas - Inteligencia Artificial
    (3, 4, 3.50, '2024-03-22'), -- Lucas - Ciberseguridad
    (4, 4, 5.50, '2024-03-22'), -- Sofía - Ciberseguridad
    (4, 5, 8.25, '2024-03-25'), -- Sofía - Comunicaciones II
    (5, 1, 5.00, '2024-03-15'), -- Mateo - Desarrollo Web
    (5, 5, 8.00, '2024-03-25'); -- Mateo - Comunicaciones II