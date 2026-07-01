CREATE DATABASE IF NOT EXISTS projects
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE projects;

DROP TABLE IF EXISTS `balanca_toledo_itens`;

CREATE TABLE `balanca_toledo_itens` (
  `tipo` char(1) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `codigo` int DEFAULT NULL,
  `validade` int DEFAULT NULL,
  `descricao` varchar(25) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cod_nutri` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TABLE IF EXISTS `balanca_toledo_nutri`;

CREATE TABLE `balanca_toledo_nutri` (
  `codigo` int DEFAULT NULL,
  `porcoes_embalagens` int DEFAULT NULL,
  `porcoes` int DEFAULT NULL,
  `unidade` char(1) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `medida_inteira` int DEFAULT NULL,
  `medida_decimal` int DEFAULT NULL,
  `medida_caseira` int DEFAULT NULL,
  `valor_energetico` int DEFAULT NULL,
  `carboidratos` decimal(5,1) DEFAULT NULL,
  `acucares_totais` decimal(5,1) DEFAULT NULL,
  `acucares_adicionados` decimal(5,1) DEFAULT NULL,
  `proteinas` decimal(5,1) DEFAULT NULL,
  `gorduras_totais` decimal(5,1) DEFAULT NULL,
  `gorduras_saturadas` decimal(5,1) DEFAULT NULL,
  `gorduras_trans` decimal(5,1) DEFAULT NULL,
  `fibras` decimal(5,1) DEFAULT NULL,
  `sodio` decimal(6,1) DEFAULT NULL,
  `alto_acucar` char(1) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `alto_gordura` char(1) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `alto_sodio` char(1) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `lactose` decimal(6,1) DEFAULT NULL,
  `galactose` decimal(6,1) DEFAULT NULL,
  `acucares_adicionados_ext` decimal(6,1) DEFAULT NULL,
  `acucares_totais_ext` decimal(6,1) DEFAULT NULL,
  `gorduras_totais_ext` decimal(6,1) DEFAULT NULL,
  `proteinas_ext` decimal(6,1) DEFAULT NULL,
  `codigo_sd` int DEFAULT NULL,
  `descricao` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;