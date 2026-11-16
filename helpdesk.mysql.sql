CREATE DATABASE IF NOT EXISTS `helpdesk`
    CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_bin;
USE `helpdesk`;

CREATE TABLE IF NOT EXISTS `Usuarios` (
    `Id` INT NOT NULL AUTO_INCREMENT,	
    `Nome` LONGTEXT NOT NULL,
    `Email` VARCHAR(254) NOT NULL,
    `SenhaHash` LONGTEXT NOT NULL,
    `Perfil` LONGTEXT NOT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_Usuarios_Email` (`Email`)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_bin;

CREATE TABLE IF NOT EXISTS `Chamados` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Titulo` LONGTEXT NOT NULL,
    `Descricao` LONGTEXT NOT NULL,
    `Status` LONGTEXT NOT NULL,
    `Prioridade` LONGTEXT NOT NULL,
    `DataCriacao` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UsuarioId` INT NOT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_Chamados_UsuarioId` (`UsuarioId`),
    CONSTRAINT `FK_Chamados_Usuarios_UsuarioId`
        FOREIGN KEY (`UsuarioId`) REFERENCES `Usuarios` (`Id`) ON DELETE NO ACTION
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_bin;

CREATE TABLE IF NOT EXISTS `Interacoes` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Mensagem` LONGTEXT NOT NULL,
    `DataCriacao` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `ChamadoId` INT NOT NULL,
    `UsuarioId` INT NOT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_Interacoes_ChamadoId` (`ChamadoId`),
    KEY `IX_Interacoes_UsuarioId` (`UsuarioId`),
    CONSTRAINT `FK_Interacoes_Chamados_ChamadoId`
        FOREIGN KEY (`ChamadoId`) REFERENCES `Chamados` (`Id`) ON DELETE NO ACTION,
    CONSTRAINT `FK_Interacoes_Usuarios_UsuarioId`
        FOREIGN KEY (`UsuarioId`) REFERENCES `Usuarios` (`Id`) ON DELETE NO ACTION
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_bin;
