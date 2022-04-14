IMAGES = api-server game-server match-server admin-server fluentd-agent fluentd-collector prometheus grafana locust

build: $(foreach IMAGE,$(IMAGES),build-$(IMAGE))

$(foreach IMAGE,$(IMAGES),build-$(IMAGE)): build-%:
	docker build \
	    --tag $(REGISTRY)/$(NAMESPACE)-$*:$(VERSION) \
	    --tag $(REGISTRY)/$(NAMESPACE)-$*:$(ENVIRONMENT) \
	    --build-arg ENVIRONMENT=$(ENVIRONMENT) \
	    $(BUILD_IMAGE_PARAM) \
	    $(NAMESPACE)-$* 

push: $(foreach IMAGE,$(IMAGES),push-$(IMAGE))

$(foreach IMAGE,$(IMAGES),push-$(IMAGE)): push-%: 
	docker push $(REGISTRY)/$(NAMESPACE)-$*:$(ENVIRONMENT)
	docker push $(REGISTRY)/$(NAMESPACE)-$*:$(VERSION)
	
tag: $(foreach IMAGE,$(IMAGES),tag-$(IMAGE))

$(foreach IMAGE,$(IMAGES),tag-$(IMAGE)): tag-%: 
	docker tag $(REGISTRY)/$(NAMESPACE)-$*:$(VERSION) $(REGISTRY)/$(NAMESPACE)-$*:$(ENVIRONMENT)
	
deploy-local: 
	docker-compose \
	    -f install/docker-compose.db.yml \
	    up
