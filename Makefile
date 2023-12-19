run: ## Run the application
	./buildscripts/run.sh

all-tests: ## Run unit tests
	./buildscripts/all-tests.sh

unit-tests: ## Run unit tests
	./buildscripts/unit-tests.sh

-include buildscripts/util.make
