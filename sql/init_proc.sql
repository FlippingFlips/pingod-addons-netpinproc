﻿BEGIN TRANSACTION;

--TODO - untested tables, old

-- Table: Machine
INSERT INTO Machine (Id, machineType, numOfBalls) VALUES (1, 7, 4);
-- Table: PRCoils
INSERT INTO Coils (Id, Name, Number, PulseTime, Bus, Polarity) VALUES ('flipperLwRMain', NULL, 'C01', 30, '', 0);
INSERT INTO Coils (Id, Name, Number, PulseTime, Bus, Polarity) VALUES ('flipperLwRHold', NULL, 'C02', 30, '', 0);
INSERT INTO Coils (Id, Name, Number, PulseTime, Bus, Polarity) VALUES ('flipperLwLMain', NULL, 'C03', 30, '', 0);
INSERT INTO Coils (Id, Name, Number, PulseTime, Bus, Polarity) VALUES ('flipperLwLHold', NULL, 'C04', 30, '', 0);
INSERT INTO Coils (Id, Name, Number, PulseTime, Bus, Polarity) VALUES ('trough',		   NULL, 'C05', 30, '', 0);
INSERT INTO Coils (Id, Name, Number, PulseTime, Bus, Polarity) VALUES ('slingL',		   NULL, 'C06', 30, '', 0);
INSERT INTO Coils (Id, Name, Number, PulseTime, Bus, Polarity) VALUES ('slingR',		   NULL, 'C07', 30, '', 0);
INSERT INTO Coils (Id, Name, Number, PulseTime, Bus, Polarity) VALUES ('bumper1',		   NULL, 'C08', 30, '', 0);
INSERT INTO Coils (Id, Name, Number, PulseTime, Bus, Polarity) VALUES ('bumper2',	       NULL, 'C09', 30, '', 0);
INSERT INTO Coils (Id, Name, Number, PulseTime, Bus, Polarity) VALUES ('bumper3',		   NULL, 'C10', 30, '', 0);
-- Table: PRLamps
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('ballSave', NULL, 'L11', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('key1', NULL, 'L12', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('key2', NULL, 'L13', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('key3', NULL, 'L14', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('key4', NULL, 'L15', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('key5', NULL, 'L16', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('b1k', NULL, 'L17', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('b2k', NULL, 'L18', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('b3k', NULL, 'L19', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('b4k', NULL, 'L20', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('b5k', NULL, 'L21', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('b6k', NULL, 'L22', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('b7k', NULL, 'L23', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('b8k', NULL, 'L24', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('b9k', NULL, 'L25', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('b10k', NULL, 'L26', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('b20k', NULL, 'L27', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('x2', NULL, 'L28', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('x3', NULL, 'L29', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('x5', NULL, 'L30', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('left25k', NULL, 'L31', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('right25k', NULL, 'L32', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('pm0', NULL, 'L33', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('pm1', NULL, 'L34', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('pm2', NULL, 'L35', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('pm3', NULL, 'L36', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('pm4', NULL, 'L37', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('xtraball', NULL, 'L38', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('specialR', NULL, 'L39', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('grot25k', NULL, 'L40', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('grot1', NULL, 'L41', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('grot2', NULL, 'L42', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('grot3', NULL, 'L43', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('grot4', NULL, 'L44', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('grot5', NULL, 'L45', '', 0);
INSERT INTO Lamps (Id, Name, Number, Bus, Polarity) VALUES ('rArrow', NULL, 'L46', '', 0);
-- Table: PRSwitches
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('coin1', NULL, 'NO', '00', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('coin2', NULL, 'NO', '01', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('coin3', NULL, 'NO', '02', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('coin4', NULL, 'NO', '03', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('exit', NULL, 'NO', '04', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('down', NULL, 'NO', '05', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('up', NULL, 'NO', '06', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('enter', NULL, 'NO', '07', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('coinDoor', NULL, 'NO', '08', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('flipperLwL', NULL, 'NO', '09', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('flipperLwR', NULL, 'NO', '11', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('flipperUpL', NULL, 'NO', '13', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('flipperUpR', NULL, 'NO', '15', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('slamTilt', NULL, 'NO', '16', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('tilt', NULL, 'NO', '17', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('ballShooter', NULL, 'NO', '18', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('startButton', NULL, 'NO', '19', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('troughEntry', NULL, 'NO', '20', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('trough1', NULL, 'NO', '21', 'trough');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('trough2', NULL, 'NO', '22', 'trough');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('trough3', NULL, 'NO', '23', 'trough');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('trough4', NULL, 'NO', '24', 'trough');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('shooter', NULL, 'NO', '25', 'shooter');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('leftOutLane', NULL, 'NO', '26', 'early_save');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('leftReturn', NULL, 'NO', '27', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('rightOutLane', NULL, 'NO', '28', 'early_save');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('rightReturn', NULL, 'NO', '29', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('slingL', NULL, 'NO', '30', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('slingR', NULL, 'NO', '31', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('bumperL', NULL, 'NO', '32', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('bumperM', NULL, 'NO', '33', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('bumperR', NULL, 'NO', '34', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('targetL1', NULL, 'NO', '35', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('targetL2', NULL, 'NO', '36', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('targetL3', NULL, 'NO', '37', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('targetL4', NULL, 'NO', '38', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('targetL5', NULL, 'NO', '39', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('dropTarget1', NULL, 'NO', '40', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('dropTarget2', NULL, 'NO', '41', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('dropTarget3', NULL, 'NO', '42', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('dropTarget4', NULL, 'NO', '43', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('dropTarget5', NULL, 'NO', '44', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('lane1', NULL, 'NO', '45', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('lane2', NULL, 'NO', '46', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('lane3', NULL, 'NO', '47', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('lane4', NULL, 'NO', '48', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('rightLoop', NULL, 'NO', '49', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('saucer', NULL, 'NO', '50', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('extraBall', NULL, 'NO', '51', '');
INSERT INTO Switches (Id, Name, SwitchType, Number, Tags, KeyboardKey) VALUES ('targetKey', NULL, 'NO', '52', '');

INSERT INTO Players (Id, Initials, Name, [Default]) VALUES (1, 'AAA', 'Default', '1');

INSERT INTO Adjustments (Id, Name, Description, Options, OptionType, ValueDefault, Value, MenuName, SubMenuName) VALUES
('ALLOW_RESTART', 'Allow Restart','Allow game restart from holding start.', '[0,1]', 1, 1, 1, 'STANDARD_ADJ', 'GENERAL'),
('ATTRACT_MUSIC', 'Attract Music','Allow music to play in attract', '[0-1]', 0, 1, 1, 'STANDARD_ADJ', 'GENERAL'),
('BALLS_PER_GAME', 'Balls Per Game','Number of balls per game 1-5', '[1-10]', 0, 3, 3, 'STANDARD_ADJ', 'GENERAL'),
('BALL_SAVE_TIME', 'Ball Save Time','Ball saver time', '[0-25]', 0, 8, 8, 'STANDARD_ADJ', 'GENERAL'),
('BALL_SEARCH_TIME', 'Ball Search Time','Timeout to search for balls and pulse coils', '[8-30]', 0, 10, 10, 'STANDARD_ADJ', 'GENERAL'),
('IDLE_SHOOTER_TIMEOUT', 'Idle Shooter Timeout','Auto launch ball if idle in plunger lane, 0 disabled', '[0,30,60,90,120,150]', 1, 60, 60, 'STANDARD_ADJ', 'GENERAL'),
('MASTER_VOL', 'Master Volume','','[0--30]', 0, -6, -6, 'STANDARD_ADJ', 'AUDIO'),
('MUSIC_VOL', 'Music Volume','','[0--30]', 0, -6, -6, 'STANDARD_ADJ', 'AUDIO'),
('VOICE_VOL', 'Voice Volume','','[0--30]', 0, -6, -6, 'STANDARD_ADJ', 'AUDIO'),
('FX_VOL', 'Sound FX Volume','','[0--30]', 0, -6, -6, 'STANDARD_ADJ', 'AUDIO'),
('MATCH_PERCENT', 'Match Percent','Match percent, 0 off', '[0-20]', 0, 5, 5, 'STANDARD_ADJ', 'GENERAL'),
('TILT_WARNINGS', 'Tilt Warnings','Number of tilt warnings before tilt', '[0-20]', 0, 3, 3, 'STANDARD_ADJ', 'GENERAL');


COMMIT TRANSACTION;