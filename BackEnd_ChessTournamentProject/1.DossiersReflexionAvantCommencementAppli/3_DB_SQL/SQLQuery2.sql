
-- =============================================
-- Auteur      : Médéric
-- Date        : 17/04/2026
-- Description : Script de création de la DB
--               pour l'application de gestion
--               de tournois d'échecs
-- Version     : 1.0
-- =============================================



-- =============================================
CREATE DATABASE asp_api_ChessTournament;
GO
USE asp_api_ChessTournament;
GO
-- =============================================



-- =============================================
DROP TABLE IF EXISTS TournamentCategory;
DROP TABLE IF EXISTS Match_;
DROP TABLE IF EXISTS Registration;
DROP TABLE IF EXISTS Player;
DROP TABLE IF EXISTS ChessClub;
DROP TABLE IF EXISTS Category;
DROP TABLE IF EXISTS Tournament;
GO
-- =============================================



-- =============================================
-- ORDRE DES TABLES A ECRIRE --

-- TABLES AVEC AUCUNE DEPENDANCE :
-- 1. TABLE Tournament
-- 2. TABLE Category
-- 3. TABLE ChessClub

-- TABLE AVEC 1 DEPENDANCE :
-- 4. TABLE Player

-- TABLES AVEC 2 DEPENDANCES :
-- 5. TABLE Registration (Tourmament - Player)
-- 6. TABLE TournamentCategory (Tourmament - Category)
-- 7. TABLE Match (Match contient => Tournament - WhitePlayer - BlackPLayer)
-- =============================================



CREATE TABLE Tournament (
	Tournament_Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	NameTournament NVARCHAR(50) NOT NULL,
	Place NVARCHAR(50) NOT NULL,
	MinNbPlayer INT NOT NULL,
	MaxNbPLayer INT NOT NULL,
    CHECK (MinNbPlayer <= MaxNbPlayer),
	MinElo INT,
	MaxElo INT,
    CHECK (MinElo IS NULL OR MaxElo IS NULL OR MinElo <= MaxElo),
	StatusTournament NVARCHAR(50) NOT NULL DEFAULT 'En attente de joueurs',
	CurrentRound INT NOT NULL DEFAULT 0,
	WomenOnly BIT NOT NULL DEFAULT 0,
	RegistrationDeadline DATETIME NOT NULL,
	CreationDate DATETIME NOT NULL DEFAULT GETDATE(),
	UpdateDate DATETIME NOT NULL DEFAULT GETDATE(),
);

CREATE TABLE Category ( 
   Category_Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
   NameCategory NVARCHAR(50) NOT NULL,
   MinAge INT NOT NULL CHECK (MinAge >= 0),
   MaxAge INT NOT NULL CHECK (MaxAge <= 110)
);

CREATE TABLE ChessClub (
	ChessClub_Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	NameChessClub NVARCHAR(50) NOT NULL,
);


---------------------------------------------------


CREATE TABLE Player (
	Player_Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Pseudo NVARCHAR(50) NOT NULL UNIQUE,
	Email NVARCHAR(50) NOT NULL UNIQUE,
	Pwd NVARCHAR(MAX) NOT NULL,
	BirthDAte DATE NOT NULL,
	Gender NVARCHAR(10) NOT NULL,
	Elo INT NOT NULL DEFAULT 1200,

	ChessClub_Id INT,
	FOREIGN KEY (ChessClub_Id) REFERENCES ChessClub(ChessClub_Id)
);


---------------------------
--|RELATION MANY-TO-MANY|--
---------------------------

CREATE TABLE Registration
(
   Registration_Id INT PRIMARY KEY IDENTITY (1,1),
   Player_Id INT NOT NULL,
   Tournament_Id INT NOT NULL,
   Wins INT DEFAULT 0,
   Losses INT DEFAULT 0,
   Draws INT DEFAULT 0,
   Score DECIMAL(4,1) DEFAULT 0,
   MatchesPlayed INT DEFAULT 0,
   RegistrationDate DATETIME DEFAULT GETDATE(),

   FOREIGN KEY (Player_Id)     REFERENCES Player(Player_Id),
   FOREIGN KEY (Tournament_Id) REFERENCES Tournament(Tournament_Id),

   UNIQUE (Player_Id, Tournament_Id)
);

CREATE TABLE Match_
(
   Match_Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
   RoundNumber INT NOT NULL,
   Result INT NULL,

   Tournament_Id INT NOT NULL,
   WhitePlayer_Id INT NOT NULL,
   BlackPlayer_Id INT NOT NULL,
   FOREIGN KEY (WhitePlayer_Id) REFERENCES Player(Player_Id),
   FOREIGN KEY (BlackPlayer_Id) REFERENCES Player(Player_Id),
   FOREIGN KEY (Tournament_Id) REFERENCES Tournament(Tournament_Id),

   UNIQUE (Tournament_Id, WhitePlayer_Id, BlackPlayer_Id)
);

CREATE TABLE TournamentCategory 
(
   Tournament_Id INT NOT NULL,
   Category_Id INT NOT NULL,

   PRIMARY KEY (Tournament_Id, Category_Id),

   FOREIGN KEY (Tournament_Id) REFERENCES Tournament(Tournament_Id),
   FOREIGN KEY (Category_Id) REFERENCES Category(Category_Id)
);
