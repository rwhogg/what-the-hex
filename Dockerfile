FROM ghcr.io/homebrew/ubuntu18.04:3.2.10
RUN brew analytics off
ENV HOMEBREW_NO_AUTO_UPDATE=1
RUN brew install wget unzip
RUN wget https://downloads.tuxfamily.org/godotengine/3.3.4/mono/Godot_v3.3.4-stable_mono_linux_headless_64.zip
RUN mkdir _godot
RUN mv Godot_v3.3.4-stable_mono_linux_headless_64.zip _godot/godot.zip
RUN unzip _godot/godot.zip
RUN rm -rf _godot
RUN mv Godot_v3.3.4-stable_mono_linux_headless_64/ _godot
RUN ln _godot/Godot_v3.3.4-stable_mono_linux_headless.64 _godot/godot
ENV GODOT="/home/linuxbrew/_godot/godot"

RUN mkdir script
COPY ./script/bootstrap.sh script
COPY ./script/Brewfile script
RUN bash script/bootstrap.sh

ENV PATH="/home/linuxbrew/_godot:/home/linuxbrew/go/bin:/home/linuxbrew/.linuxbrew/bin:/home/linuxbrew/.linuxbrew/sbin:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin"
ENV DOTNET_CLI_TELEMETRY_OPTOUT="true"
