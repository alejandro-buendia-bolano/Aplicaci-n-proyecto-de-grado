INSERT INTO Registro VALUES (1081586053, 57291716, '2026-08-04', '09:00:07');
INSERT INTO Registro VALUES (1082820592, 57291716, '2026-08-04', '09:00:15');
INSERT INTO Registro VALUES (1094752529, 57291716, '2026-08-04', '09:00:21');
INSERT INTO Registro VALUES (1055833156, 57291716, '2026-08-04', '09:00:27');
INSERT INTO Registro VALUES (1098961459, 57291716, '2026-08-04', '09:00:36');
INSERT INTO Registro VALUES (1098320463, 57291716, '2026-08-04', '09:00:46');
INSERT INTO Registro VALUES (1052857966, 57291716, '2026-08-04', '09:00:54');
INSERT INTO Registro VALUES (1064349361, 57291716, '2026-08-04', '09:01:00');
INSERT INTO Registro VALUES (1093793389, 57291716, '2026-08-04', '09:01:10');
INSERT INTO Registro VALUES (1068181396, 57291716, '2026-08-04', '09:01:17');
INSERT INTO Registro VALUES (1076792612, 57291716, '2026-08-04', '09:01:21');
INSERT INTO Registro VALUES (1051244663, 57291716, '2026-08-04', '09:01:31');
INSERT INTO Registro VALUES (1095345555, 57291716, '2026-08-04', '09:01:37');
INSERT INTO Registro VALUES (1096149359, 57291716, '2026-08-04', '09:01:43');
INSERT INTO Registro VALUES (1059349722, 57291716, '2026-08-04', '09:01:48');
INSERT INTO Registro VALUES (1068718453, 57291716, '2026-08-04', '09:01:54');
INSERT INTO Registro VALUES (1093140807, 57291716, '2026-08-04', '09:02:00');
INSERT INTO Registro VALUES (1086238574, 57291716, '2026-08-04', '09:02:09');
INSERT INTO Registro VALUES (1068966946, 57291716, '2026-08-04', '09:02:18');
INSERT INTO Registro VALUES (1045982341, 57291716, '2026-08-04', '09:02:24');
INSERT INTO Registro VALUES (1073551839, 57291716, '2026-08-04', '09:02:30');
INSERT INTO Registro VALUES (1049876321, 57291716, '2026-08-04', '09:02:37');
INSERT INTO Registro VALUES (1091971316, 57291716, '2026-08-04', '09:02:43');
INSERT INTO Registro VALUES (1096125617, 57291716, '2026-08-04', '09:02:53');
INSERT INTO Registro VALUES (1065657579, 57291716, '2026-08-04', '09:03:03');
INSERT INTO Registro VALUES (1047654321, 57291716, '2026-08-04', '09:03:11');
INSERT INTO Registro VALUES (1048898923, 57291716, '2026-08-04', '09:03:18');
INSERT INTO Registro VALUES (1091182864, 57291716, '2026-08-04', '09:03:25');

-----------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Aula_Docente')
CREATE TABLE Aula_Docente (
    idAula    INT NOT NULL,
    idDocente INT NOT NULL, -- Cambiado a INT para que coincida con la tabla Docente
    PRIMARY KEY (idAula, idDocente),
    FOREIGN KEY (idAula)    REFERENCES Aula(idAula),
    FOREIGN KEY (idDocente) REFERENCES Docente(idDocente)
);

SELECT * FROM AulaEstudiante where idAula = 1;


-- Paso 1: ver qué columna es
SELECT c.name AS ColumnaAfectada
FROM sys.key_constraints kc
JOIN sys.index_columns ic ON ic.object_id = kc.parent_object_id
                         AND ic.index_id  = kc.unique_index_id
JOIN sys.columns c        ON c.object_id = ic.object_id
                         AND c.column_id = ic.column_id
WHERE kc.name = 'UQ__Docente__E3237CD7568068F7';

-- Paso 2: reemplazar la restricción por un índice único filtrado
DECLARE @col sysname, @sql nvarchar(max);

SELECT TOP 1 @col = c.name
FROM sys.key_constraints kc
JOIN sys.index_columns ic ON ic.object_id = kc.parent_object_id
                         AND ic.index_id  = kc.unique_index_id
JOIN sys.columns c        ON c.object_id = ic.object_id
                         AND c.column_id = ic.column_id
WHERE kc.name = 'UQ__Docente__E3237CD7568068F7';

SET @sql = N'ALTER TABLE dbo.Docente DROP CONSTRAINT UQ__Docente__E3237CD7568068F7;
CREATE UNIQUE INDEX UX_Docente_' + @col + N'
    ON dbo.Docente([' + @col + N']) WHERE [' + @col + N'] IS NOT NULL;';

EXEC sp_executesql @sql;