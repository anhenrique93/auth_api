--Create
CREATE TABLE "USER" (
    Id SERIAL NOT NULL, 
    Email varchar(255) NOT NULL UNIQUE, 
    Password bytea NOT NULL,
    PasswordSalt bytea, 
    RefreshToken varchar(255),
    TokenCreated timestamp without time zone,
    TokenExpires timestamp without time zone,
    PRIMARY KEY (Id)
);


ALTER TABLE "USER"
ADD COLUMN PasswordSalt bytea;

ALTER TABLE "USER"
ADD COLUMN RefreshToken varchar(255);

ALTER TABLE "USER"
ADD COLUMN TokenCreated timestamp without time zone;

ALTER TABLE "USER"
ADD COLUMN TokenExpires timestamp without time zone;

CREATE TABLE "USER" (
    Id SERIAL NOT NULL,
    FirstName varchar(255) NOT NULL,
    LastName varchar(255) NOT NULL,
    Email varchar(255) NOT NULL UNIQUE,
    Phone varchar(255),
    Address varchar(255),
    GenderId int4,
    PRIMARY KEY (Id),
    CONSTRAINT FKUSER309022 FOREIGN KEY (GenderId) REFERENCES Gender (Id)
);

--Drop
DROP TABLE IF EXISTS "USER" CASCADE;

--Select
SELECT Id, Email, Password FROM "USER";

--Insert
INSERT INTO "USER"(Id, Email, Password) VALUES (?, ?, ?);

--Update
UPDATE "USER" SET Email = ?, Password = ? WHERE Id = ?;

--Delete
DELETE FROM "USER" WHERE Id = ?;

--Stored rocedures and functions
--Register
CREATE OR REPLACE PROCEDURE registerUser(
    _email VARCHAR(255),
    _password BYTEA
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO "USER" (Email, Password) VALUES (_email, _password);
END;
$$;

--GetByEmail
CREATE OR REPLACE FUNCTION getUserByEmail(_email VARCHAR(255))
RETURNS TABLE(Id INTEGER, Email VARCHAR(255), Password BYTEA, PasswordSalt BYTEA)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT Id, Email, Password, PasswordSalt FROM "USER" WHERE Email = _email;
END;
$$;

--GetUserById
CREATE OR REPLACE FUNCTION getUserById(p_id INT)
RETURNS TABLE(
    Id INT,
    Email VARCHAR(255),
    Password BYTEA,
    PasswordSalt BYTEA,
    RefreshToken VARCHAR(255),
    TokenCreated TIMESTAMP,
    TokenExpires TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT * FROM "USER" WHERE "USER".Id = p_id;
END;
$$;

--Refresh token
CREATE OR REPLACE PROCEDURE refreshUserToken(
    p_id INT,
    p_refreshToken VARCHAR(255),
    p_tokenCreated timestamp without time zone,
    p_tokenExpires timestamp without time zone
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE "USER" 
    SET RefreshToken = p_refreshToken,
        TokenCreated = p_tokenCreated,
        TokenExpires = p_tokenExpires
    WHERE Id = p_id;
END;
$$;

--Update Email
CREATE OR REPLACE PROCEDURE updateEmail(
    _id INT,
    _email VARCHAR(255)
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE "USER"
    SET Email = _email
    WHERE Id = _id;
END;
$$;

--update Password
CREATE OR REPLACE PROCEDURE changePassword(
    _id INT,
    _password BYTEA,
    _salt BYTEA
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE "USER"
    SET Password = _password,
        PasswordSalt = _salt
    WHERE Id = _id;
END;
$$;

--Delete User
CREATE OR REPLACE PROCEDURE deleteUser(
    _id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM "USER"
    WHERE Id = _id;
END;
$$;







