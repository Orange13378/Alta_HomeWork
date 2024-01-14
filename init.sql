--
-- PostgreSQL database dump
--

-- Dumped from database version 16.0
-- Dumped by pg_dump version 16.0

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    id integer NOT NULL,
    username character varying(255) NOT NULL,
    password character varying(255) NOT NULL,
    role character varying(50) NOT NULL,
    shopid integer
);


ALTER TABLE public.users OWNER TO postgres;

--
-- Name: Users_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Users_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Users_id_seq" OWNER TO postgres;

--
-- Name: Users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Users_id_seq" OWNED BY public.users.id;


--
-- Name: logs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.logs (
    id integer NOT NULL,
    loglevel text,
    message text,
    createdat timestamp without time zone
);


ALTER TABLE public.logs OWNER TO postgres;

--
-- Name: logs_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.logs_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.logs_id_seq OWNER TO postgres;

--
-- Name: logs_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.logs_id_seq OWNED BY public.logs.id;


--
-- Name: shops; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.shops (
    id integer NOT NULL,
    createdate timestamp without time zone NOT NULL,
    changedate timestamp without time zone NOT NULL,
    name character varying(255) NOT NULL,
    code character varying(20) NOT NULL,
    address text,
    phone character varying(15),
    typeid integer,
    login character varying(50) NOT NULL,
    enabled boolean NOT NULL
);


ALTER TABLE public.shops OWNER TO postgres;

--
-- Name: shop_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.shop_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.shop_id_seq OWNER TO postgres;

--
-- Name: shop_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.shop_id_seq OWNED BY public.shops.id;


--
-- Name: shoptypes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.shoptypes (
    id integer NOT NULL,
    name character varying(255) NOT NULL,
    comment text
);


ALTER TABLE public.shoptypes OWNER TO postgres;

--
-- Name: shoptypes_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.shoptypes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.shoptypes_id_seq OWNER TO postgres;

--
-- Name: shoptypes_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.shoptypes_id_seq OWNED BY public.shoptypes.id;


--
-- Name: logs id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.logs ALTER COLUMN id SET DEFAULT nextval('public.logs_id_seq'::regclass);


--
-- Name: shops id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shops ALTER COLUMN id SET DEFAULT nextval('public.shop_id_seq'::regclass);


--
-- Name: shoptypes id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shoptypes ALTER COLUMN id SET DEFAULT nextval('public.shoptypes_id_seq'::regclass);


--
-- Name: users id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public."Users_id_seq"'::regclass);


--
-- Data for Name: logs; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.logs (id, loglevel, message, createdat) FROM stdin;
779	Info	Hosting environment: Development	2023-11-22 22:05:46.0116
785	Info	Request finished HTTP/1.1 GET http://localhost:5217/swagger/index.html - - - 200 - text/html;charset=utf-8 307.9531ms	2023-11-22 22:06:03.6542
789	Info	Request starting HTTP/1.1 GET http://localhost:5217/_vs/browserLink - -	2023-11-22 22:06:03.8149
795	Info	Request finished HTTP/1.1 GET http://localhost:5217/swagger/swagger-ui-standalone-preset.js - - - 200 312163 text/javascript 39.5545ms	2023-11-22 22:06:03.8559
800	Info	Request finished HTTP/1.1 GET http://localhost:5217/swagger/v1/swagger.json - - - 200 - application/json;charset=utf-8 279.0006ms	2023-11-22 22:06:06.6259
766	Info	Executing StatusCodeResult, setting HTTP status code 200	2023-11-13 10:11:44.5366
770	Info	Request starting HTTP/1.1 GET http://localhost:5217/api/Logs/logs - -	2023-11-13 10:11:49.5277
775	Info	Executed endpoint 'DapperHomeWork.Controllers.LogsController.GetLogs (DapperHomeWork)'	2023-11-13 10:11:49.5277
777	Info	Content root path: D:\\Programer\\DapperHomeWork\\DapperHomeWork	2023-11-22 22:05:46.0116
783	Info	Request finished HTTP/1.1 GET http://localhost:5217/swagger - - - 301 0 - 207.3268ms	2023-11-22 22:06:03.3311
788	Info	Request finished HTTP/1.1 GET http://localhost:5217/swagger/swagger-ui.css - - - 200 144929 text/css 29.4863ms	2023-11-22 22:06:03.6837
793	Info	Request finished HTTP/1.1 GET http://localhost:5217/_framework/aspnetcore-browser-refresh.js - - - 200 12360 application/javascript;+charset=utf-8 2.9917ms	2023-11-22 22:06:03.8149
794	Info	Sending file. Request path: '/swagger-ui-standalone-preset.js'. Physical path: 'N/A'	2023-11-22 22:06:03.8403
799	Info	Request starting HTTP/1.1 GET http://localhost:5217/swagger/v1/swagger.json - -	2023-11-22 22:06:06.3469
804	Info	Application is shutting down...	2023-11-22 22:08:43.014
780	Info	User profile is available. Using 'C:\\Users\\musli\\AppData\\Local\\ASP.NET\\DataProtection-Keys' as key repository and Windows DPAPI to encrypt keys at rest.	2023-11-22 22:05:44.9756
786	Info	Request starting HTTP/1.1 GET http://localhost:5217/swagger/swagger-ui.css - -	2023-11-22 22:06:03.6542
791	Info	Request starting HTTP/1.1 GET http://localhost:5217/swagger/swagger-ui-standalone-preset.js - -	2023-11-22 22:06:03.8149
798	Info	Request finished HTTP/1.1 GET http://localhost:5217/_vs/browserLink - - - 200 - text/javascript;+charset=UTF-8 2426.1665ms	2023-11-22 22:06:06.2433
803	Info	Request finished HTTP/1.1 GET http://localhost:5217/swagger/favicon-32x32.png - - - 200 628 image/png 22.7588ms	2023-11-22 22:06:07.2764
778	Info	Application started. Press Ctrl+C to shut down.	2023-11-22 22:05:46.0116
784	Info	Request starting HTTP/1.1 GET http://localhost:5217/swagger/index.html - -	2023-11-22 22:06:03.3393
790	Info	Request starting HTTP/1.1 GET http://localhost:5217/swagger/swagger-ui-bundle.js - -	2023-11-22 22:06:03.8149
796	Info	Sending file. Request path: '/swagger-ui-bundle.js'. Physical path: 'N/A'	2023-11-22 22:06:03.8742
802	Info	Sending file. Request path: '/favicon-32x32.png'. Physical path: 'N/A'	2023-11-22 22:06:07.2734
781	Info	Now listening on: http://localhost:5217	2023-11-22 22:05:46.008
782	Info	Request starting HTTP/1.1 GET http://localhost:5217/swagger - -	2023-11-22 22:06:03.1256
787	Info	Sending file. Request path: '/swagger-ui.css'. Physical path: 'N/A'	2023-11-22 22:06:03.6677
792	Info	Request starting HTTP/1.1 GET http://localhost:5217/_framework/aspnetcore-browser-refresh.js - -	2023-11-22 22:06:03.8149
797	Info	Request finished HTTP/1.1 GET http://localhost:5217/swagger/swagger-ui-bundle.js - - - 200 1061536 text/javascript 61.1642ms	2023-11-22 22:06:03.8742
801	Info	Request starting HTTP/1.1 GET http://localhost:5217/swagger/favicon-32x32.png - -	2023-11-22 22:06:07.2563
768	Info	Executed endpoint 'DapperHomeWork.Controllers.LogsController.DeleteLogs (DapperHomeWork)'	2023-11-13 10:11:44.5366
772	Info	Route matched with {action = "GetLogs", controller = "Logs"}. Executing controller action with signature Microsoft.AspNetCore.Mvc.IActionResult GetLogs() on controller DapperHomeWork.Controllers.LogsController (DapperHomeWork).	2023-11-13 10:11:49.5277
765	Info	Route matched with {action = "DeleteLogs", controller = "Logs"}. Executing controller action with signature Microsoft.AspNetCore.Mvc.IActionResult DeleteLogs() on controller DapperHomeWork.Controllers.LogsController (DapperHomeWork).	2023-11-13 10:11:44.5055
774	Info	Executed action DapperHomeWork.Controllers.LogsController.GetLogs (DapperHomeWork) in 2.4902ms	2023-11-13 10:11:49.5277
767	Info	Executed action DapperHomeWork.Controllers.LogsController.DeleteLogs (DapperHomeWork) in 37.7682ms	2023-11-13 10:11:44.5366
771	Info	Executing endpoint 'DapperHomeWork.Controllers.LogsController.GetLogs (DapperHomeWork)'	2023-11-13 10:11:49.5277
776	Info	Request finished HTTP/1.1 GET http://localhost:5217/api/Logs/logs - - - 200 - application/json;+charset=utf-8 6.2749ms	2023-11-13 10:11:49.5277
764	Info	Executing endpoint 'DapperHomeWork.Controllers.LogsController.DeleteLogs (DapperHomeWork)'	2023-11-13 10:11:44.4909
769	Info	Request finished HTTP/1.1 DELETE http://localhost:5217/api/Logs - - - 200 0 - 63.4755ms	2023-11-13 10:11:44.5366
773	Info	Executing OkObjectResult, writing value of type 'System.Collections.Generic.List`1[[DapperHomeWork.Models.Log.Log, DapperHomeWork, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]'.	2023-11-13 10:11:49.5277
\.


--
-- Data for Name: shops; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.shops (id, createdate, changedate, name, code, address, phone, typeid, login, enabled) FROM stdin;
1	2023-11-10 13:49:13.297	2023-11-10 13:49:13.297	string	string	string	string	1	string	t
2	2023-11-12 15:35:43.024	2023-11-12 15:35:43.024	asdaw	12312321	asdwaasdw	123455463	2	login	t
\.


--
-- Data for Name: shoptypes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.shoptypes (id, name, comment) FROM stdin;
1	Товар	\N
2	Услуга	\N
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (id, username, password, role, shopid) FROM stdin;
1	string	$2a$11$sFdNOdG4WJcz2pnyTK1IZOMKbNGarCaKLZYEydjAusVhmVL21w8Xe	admin	\N
2	admin	$2a$11$J6zh9/MgLmE5T3UmRJf93OofIWfaW6HQqq2h71xWJYU6lTKNsCKdu	user	\N
\.


--
-- Name: Users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Users_id_seq"', 2, true);


--
-- Name: logs_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.logs_id_seq', 804, true);


--
-- Name: shop_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.shop_id_seq', 2, true);


--
-- Name: shoptypes_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.shoptypes_id_seq', 1, false);


--
-- Name: users Users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT "Users_pkey" PRIMARY KEY (id);


--
-- Name: logs logs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.logs
    ADD CONSTRAINT logs_pkey PRIMARY KEY (id);


--
-- Name: shops shop_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shops
    ADD CONSTRAINT shop_pkey PRIMARY KEY (id);


--
-- Name: shoptypes shoptypes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shoptypes
    ADD CONSTRAINT shoptypes_pkey PRIMARY KEY (id);


--
-- Name: idx_shop_code; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_shop_code ON public.shops USING btree (code);


--
-- Name: idx_shop_login; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_shop_login ON public.shops USING btree (login);


--
-- Name: idx_shop_name; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_shop_name ON public.shops USING btree (name);


--
-- Name: idx_shop_phone; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_shop_phone ON public.shops USING btree (phone);


--
-- Name: idx_shops_address; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_shops_address ON public.shops USING btree (address);


--
-- Name: idx_shoptypes_name; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_shoptypes_name ON public.shoptypes USING btree (name);


--
-- Name: idx_users_role; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_users_role ON public.users USING btree (role);


--
-- Name: idx_users_username; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_users_username ON public.users USING btree (username);


--
-- Name: users Users_shopid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT "Users_shopid_fkey" FOREIGN KEY (shopid) REFERENCES public.shops(id);


--
-- Name: shops shop_typeid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shops
    ADD CONSTRAINT shop_typeid_fkey FOREIGN KEY (typeid) REFERENCES public.shoptypes(id);


--
-- PostgreSQL database dump complete
--

