CREATE DATABASE RaceDay;
USE RaceDay;

CREATE TABLE Organiser (
OrganiserID int PRIMARY KEY,
Name VARCHAR(100) NOT NULL,
Email VARCHAR(255) NOT NULL,
EventID int,
CONSTRAINT fk_Event
FOREIGN KEY (EventID) 
REFERENCES Event(EventID)
);
CREATE TABLE Event (
EventID int PRIMARY KEY,
EventName VARCHAR(255) NOT NULL,
Date VARCHAR(255) NOT NULL,
Location VARCHAR(255) NOT NULL ,
Description VARCHAR(255) NOT NULL,
Distance VARCHAR(255) NOT NULL ,

OrganiserID int,
CONSTRAINT fk_Organiser
FOREIGN KEY (OrganiserID) 
REFERENCES Organiser(OrganiserID),

CatergoryID int,
CONSTRAINT fk_Catergory
FOREIGN KEY (CatergoryID) 
REFERENCES Catergory(CatergoryID)
);

CREATE TABLE Participants (
ParticipantID int PRIMARY KEY,
Name VARCHAR(255) NOT NULL,
PhoneNumber VARCHAR(255) NOT NULL ,
Gender VARCHAR(55) NOT NULL,
DateOfBirth VARCHAR(255) NOT NULL,

EventID int,
CONSTRAINT fk_Event
FOREIGN KEY (EventID) 
REFERENCES Event(EventID)
);

CREATE TABLE Results (
EventID int PRIMARY KEY,
FinishTime VARCHAR(255) NOT NULL,
FinsihPosition VARCHAR(255) NOT NULL,
EventID int,
CONSTRAINT fk_Event
FOREIGN KEY (EventID) 
REFERENCES Event(EventID),

CatergoryID int,
CONSTRAINT fk_Catergory
FOREIGN KEY (CatergoryID) 
REFERENCES Catergory(CatergoryID),

ParticipantID int,
CONSTRAINT fk_Participant
FOREIGN KEY (ParticipantID) 
REFERENCES Participants(ParticipantID)
);
CREATE TABLE Catergory (
CatergoryID int PRIMARY KEY,
CatergoryType VARCHAR(255) NOT NULL,
EventID int,
CONSTRAINT fk_Event
FOREIGN KEY (EventID) 
REFERENCES Event(EventID)

);
CREATE TABLE  Enrolment (
EnrolmentID int PRIMARY KEY,

EventID int,
CONSTRAINT fk_Event
FOREIGN KEY (EventID) 
REFERENCES Event(EventID),

ParticipantID int,
CONSTRAINT fk_Participant
FOREIGN KEY (ParticipantID) 
REFERENCES Participants(ParticipantID),

CatergoryID int,
CONSTRAINT fk_Catergory
FOREIGN KEY (CatergoryID) 
REFERENCES Catergory(CatergoryID),
);