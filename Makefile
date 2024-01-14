rm:
	docker-compose stop \
	&& docker-compose rm \
	&& rm -rf pgdata/
	
up:
	docker-compose -f docker-compose_init.yml up --force-recreate
	