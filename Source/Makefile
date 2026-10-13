.PHONY: build deploy clean check-dll compare-dll deploy-dll install

rwproj=/Volumes/expand/projects/rw
modname=CatsAreCats
steam_mods_dir=/Users/cem/Library/Application\ Support/Steam/steamapps/common/RimWorld/Mods
dll_source=obj/Release/net48/CatsAreCats.dll
dll_target_16=../1.6/Assemblies/CatsAreCats.dll
dll_target_17=../1.7/Assemblies/CatsAreCats.dll

check-dll:
	@echo "=== Checking DLL status ==="
	@if [ -f "${dll_source}" ]; then \
		echo "Source DLL: ${dll_source}"; \
		ls -la "${dll_source}"; \
	else \
		echo "Source DLL not found: ${dll_source}"; \
	fi
	@for f in "${dll_target_16}" "${dll_target_17}"; do \
		if [ -f "$$f" ]; then echo "Target DLL: $$f"; ls -la "$$f"; else echo "Target DLL not found: $$f"; fi; \
	done

compare-dll: check-dll
	@echo "=== Comparing DLLs ==="
	@if [ ! -f "${dll_source}" ]; then echo "Source DLL not found - build needed"; \
	else \
		for f in "${dll_target_16}" "${dll_target_17}"; do \
			if [ ! -f "$$f" ]; then echo "Source exists but target missing: $$f"; \
			elif cmp -s "${dll_source}" "$$f"; then echo "Identical: $$f"; \
			else echo "DLLs differ: $$f"; fi; \
		done; \
	fi

build:
	@echo "=== Building ${modname} ==="
	cd ${rwproj}/${modname}/Source ; \
	dotnet build "${modname}".csproj --configuration Release
	@echo "Build completed. Checking output..."

deploy-dll: build
	@echo "=== Deploying DLL ==="
	@if [ -f "${dll_source}" ]; then \
		mkdir -p ../1.6/Assemblies ../1.7/Assemblies; \
		cp "${dll_source}" "${dll_target_16}"; \
		cp "${dll_source}" "${dll_target_17}"; \
		echo "DLL deployed to ${dll_target_16} and ${dll_target_17}"; \
		ls -la "${dll_target_16}" "${dll_target_17}"; \
	else \
		echo "Source DLL not found: ${dll_source}"; \
		exit 1; \
	fi

deploy: deploy-dll
	@echo "=== Deploying mod to RimWorld ==="
	cd ${rwproj} ; pwd ; tar cpf - ${modname} | (cd ${steam_mods_dir} && tar xf -)
	@echo "Mod deployed to RimWorld"

install: deploy

clean:
	@echo "=== Cleaning build artifacts ==="
	rm -rf obj bin
	@rm -f "${dll_target_16}" "${dll_target_17}"
	@echo "Clean completed"
