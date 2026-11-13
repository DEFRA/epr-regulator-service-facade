#!/bin/sh
# Upgrade nuget packages to latest, then run the tests and commit.
# Run upgrade-dotnet.sh first so the dotnet major is current.
set -e # exit on error

if ! git diff --cached --quiet; then
  echo "Aborted: staged changes detected in git"
  exit 1
fi

if ! command -v dotnet-outdated
then
	# https://github.com/dotnet-outdated/dotnet-outdated
	dotnet tool install --global dotnet-outdated-tool
fi

# Packages versioned with the runtime are capped at its major. Many also
# target netstandard, so dotnet-outdated would otherwise offer e.g. 11.x
# on net10.0. The maximum is inclusive and needs at least major.minor, so
# a huge minor means any 10.x; omitted parts are treated as unbounded.
dotnet_major=$(sed -n 's/^dotnet = "\([0-9]*\)\..*/\1/p' mise.toml)

# --no-restore because dotnet-outdated otherwise restores each project as it
# edits it, and a project restored before the projects it references are
# edited fails with a spurious downgrade (NU1605). dotnet test below
# restores the finished result.
dotnet outdated -u --no-restore --maximum-version "$dotnet_major.99999" \
  -inc Microsoft.AspNetCore -inc Microsoft.Extensions regulator_facade.sln
# Held back:
# - ApplicationInsights 3 stops the app starting: the logging that feeds it,
#   Serilog.Sinks.ApplicationInsights and
#   Microsoft.Extensions.Logging.ApplicationInsights (no 3.x release), both
#   require Microsoft.ApplicationInsights [2.23.0, 3.0.0).
#   https://www.nuget.org/packages/Serilog.Sinks.ApplicationInsights#dependencies-body-tab
#   https://www.nuget.org/packages/Microsoft.Extensions.Logging.ApplicationInsights
# - Swashbuckle 10 moves to Microsoft.OpenApi 2, whose model changes break
#   the Swagger filters in the API project.
#   https://github.com/domaindrivendev/Swashbuckle.AspNetCore/releases/tag/v10.0.0
# Moving these needs code changes, so is left for a separate change.
dotnet outdated -u --no-restore -exc Microsoft.AspNetCore -exc Microsoft.Extensions \
  -exc Microsoft.ApplicationInsights -exc Swashbuckle regulator_facade.sln

if git diff --quiet -- 'src/*.csproj' Directory.Build.props; then
  echo "No package updates available, nothing to do"
  exit 0
fi

echo "Running regression tests..."
dotnet test regulator_facade.sln

echo "Committing upgrades..."
# shellcheck disable=SC2046
git commit $(git diff --name-only -- 'src/*.csproj' Directory.Build.props | sed 's/^/--include /') \
--message "chore: Dependency upgrades (nuget)" \
--message "Done with 'upgrade-nugets.sh'"
