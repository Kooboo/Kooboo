using System;
using System.Runtime.InteropServices;
using LibSass.Compiler.Context;
using LibSass.Compiler.Options;
using LibSass.Types;

// ReSharper disable InconsistentNaming

// Suggestion to avoid this from happening in future versions:
// https://resharper-support.jetbrains.com/hc/en-us/community/posts/206675599

namespace LibSass.Compiler
{
    internal class SassExterns
    {
        internal static IntPtr sass_make_map(int @length)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_map(@length)
                : SassExterns64.sass_make_map(@length);
        }

        internal static IntPtr sass_map_set_key(IntPtr @value_map, int @index, IntPtr @key)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_map_set_key(@value_map, @index, @key)
                : SassExterns64.sass_map_set_key(@value_map, @index, @key);
        }

        internal static IntPtr sass_map_set_value(IntPtr @value_map, int @index, IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_map_set_value(@value_map, @index, @value)
                : SassExterns64.sass_map_set_value(@value_map, @index, @value);
        }

        internal static IntPtr sass_make_list(int @length, Types.SassListSeparator @sep)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_list(@length, @sep)
                : SassExterns64.sass_make_list(@length, @sep);
        }

        internal static void sass_list_set_value(IntPtr @value_list, int @index, IntPtr @value)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_list_set_value(@value_list, @index, @value);
            else
                SassExterns64.sass_list_set_value(@value_list, @index, @value);
        }

        internal static IntPtr sass_make_color(double @r, double @g, double @b, double @a)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_color(@r, @g, @b, @a)
                : SassExterns64.sass_make_color(@r, @g, @b, @a);
        }

        internal static double sass_color_get_r(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_color_get_r(@value)
                : SassExterns64.sass_color_get_r(@value);
        }

        internal static double sass_color_get_g(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_color_get_g(@value)
                : SassExterns64.sass_color_get_g(@value);
        }

        internal static double sass_color_get_b(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_color_get_b(@value)
                : SassExterns64.sass_color_get_b(@value);
        }

        internal static double sass_color_get_a(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_color_get_a(@value)
                : SassExterns64.sass_color_get_a(@value);
        }

        internal static IntPtr sass_make_number(double @value, SassSafeStringHandle @unit)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_number(@value, @unit)
                : SassExterns64.sass_make_number(@value, @unit);
        }

        internal static IntPtr sass_make_boolean(bool @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_boolean(@value)
                : SassExterns64.sass_make_boolean(@value);
        }

        internal static IntPtr sass_make_string(SassSafeStringHandle @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_string(@value)
                : SassExterns64.sass_make_string(@value);
        }

        internal static IntPtr sass_string_get_value(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_string_get_value(@value)
                : SassExterns64.sass_string_get_value(@value);
        }

        internal static bool sass_boolean_get_value(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_boolean_get_value(@value)
                : SassExterns64.sass_boolean_get_value(@value);
        }

        internal static IntPtr sass_make_null()
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_null()
                : SassExterns64.sass_make_null();
        }

        internal static IntPtr sass_delete_data_context(IntPtr @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_delete_data_context(@context)
                : SassExterns64.sass_delete_data_context(@context);
        }

        internal static IntPtr sass_delete_file_context(IntPtr @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_delete_file_context(@context)
                : SassExterns64.sass_delete_file_context(@context);
        }

        internal static IntPtr sass_copy_c_string(SassSafeStringHandle @input_string)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_copy_c_string(@input_string)
                : SassExterns64.sass_copy_c_string(@input_string);
        }

        internal static int sass_compile_data_context(SassSafeDataContextHandle @data_context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_compile_data_context(@data_context)
                : SassExterns64.sass_compile_data_context(@data_context);
        }

        internal static int sass_compile_file_context(SassSafeFileContextHandle @file_context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_compile_file_context(@file_context)
                : SassExterns64.sass_compile_file_context(@file_context);
        }

        internal static IntPtr sass_make_file_context(SassSafeStringHandle @source_string)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_file_context(@source_string)
                : SassExterns64.sass_make_file_context(@source_string);
        }

        internal static IntPtr sass_make_data_context(IntPtr @source_string)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_data_context(@source_string)
                : SassExterns64.sass_make_data_context(@source_string);
        }

        internal static IntPtr sass_context_get_options(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_options(@context)
                : SassExterns64.sass_context_get_options(@context);
        }

        internal static IntPtr sass_context_get_output_string(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_output_string(@context)
                : SassExterns64.sass_context_get_output_string(@context);
        }

        internal static int sass_context_get_error_status(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_error_status(@context)
                : SassExterns64.sass_context_get_error_status(@context);
        }

        internal static IntPtr sass_context_get_error_json(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_error_json(@context)
                : SassExterns64.sass_context_get_error_json(@context);
        }

        internal static IntPtr sass_context_get_error_text(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_error_text(@context)
                : SassExterns64.sass_context_get_error_text(@context);
        }

        internal static IntPtr sass_context_get_error_message(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_error_message(@context)
                : SassExterns64.sass_context_get_error_message(@context);
        }

        internal static IntPtr sass_context_get_error_file(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_error_file(@context)
                : SassExterns64.sass_context_get_error_file(@context);
        }

        internal static IntPtr sass_context_get_error_src(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_error_src(@context)
                : SassExterns64.sass_context_get_error_src(@context);
        }

        internal static int sass_context_get_error_line(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_error_line(@context)
                : SassExterns64.sass_context_get_error_line(@context);
        }

        internal static int sass_context_get_error_column(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_error_column(@context)
                : SassExterns64.sass_context_get_error_column(@context);
        }

        internal static IntPtr sass_context_get_source_map_string(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_source_map_string(@context)
                : SassExterns64.sass_context_get_source_map_string(@context);
        }

        internal static IntPtr sass_context_get_included_files(SassSafeContextHandle @context)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_context_get_included_files(@context)
                : SassExterns64.sass_context_get_included_files(@context);
        }

        internal static IntPtr sass_make_importer(SassImporterDelegate @importer_fn, double @priority, IntPtr @cookie)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_importer(@importer_fn, @priority, @cookie)
                : SassExterns64.sass_make_importer(@importer_fn, @priority, @cookie);
        }

        internal static IntPtr sass_make_import_entry(SassSafeStringHandle @path, IntPtr @source, IntPtr @srcmap)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_import_entry(@path, @source, @srcmap)
                : SassExterns64.sass_make_import_entry(@path, @source, @srcmap);
        }

        internal static IntPtr sass_make_importer_list(int @length)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_importer_list(@length)
                : SassExterns64.sass_make_importer_list(@length);
        }

        internal static IntPtr sass_make_import_list(int @length)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_import_list(@length)
                : SassExterns64.sass_make_import_list(@length);
        }

        internal static IntPtr sass_make_function_list(int @length)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_function_list(@length)
                : SassExterns64.sass_make_function_list(@length);
        }

        internal static short sass_list_get_separator(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_list_get_separator(@value)
                : SassExterns64.sass_list_get_separator(@value);
        }

        internal static IntPtr sass_make_function(SassSafeStringHandle @signature, SassFunctionDelegate @cb, IntPtr @cookie)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_function(@signature, @cb, @cookie)
                : SassExterns64.sass_make_function(@signature, @cb, @cookie);
        }

        internal static IntPtr sass_importer_get_cookie(IntPtr @cb)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_importer_get_cookie(@cb)
                : SassExterns64.sass_importer_get_cookie(@cb);
        }

        internal static SassTag sass_value_get_tag(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_value_get_tag(@value)
                : SassExterns64.sass_value_get_tag(@value);
        }

        internal static int sass_list_get_length(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_list_get_length(@value)
                : SassExterns64.sass_list_get_length(@value);
        }

        internal static IntPtr sass_list_get_value(IntPtr @value, int @index)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_list_get_value(@value, @index)
                : SassExterns64.sass_list_get_value(@value, @index);
        }

        internal static int sass_map_get_length(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_map_get_length(@value)
                : SassExterns64.sass_map_get_length(@value);
        }

        internal static IntPtr sass_map_get_key(IntPtr @value, int @index)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_map_get_key(@value, @index)
                : SassExterns64.sass_map_get_key(@value, @index);
        }

        internal static IntPtr sass_map_get_value(IntPtr @value, int @index)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_map_get_value(@value, @index)
                : SassExterns64.sass_map_get_value(@value, @index);
        }

        internal static double sass_number_get_value(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_number_get_value(@value)
                : SassExterns64.sass_number_get_value(@value);
        }

        internal static IntPtr sass_number_get_unit(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_number_get_unit(@value)
                : SassExterns64.sass_number_get_unit(@value);
        }

        internal static IntPtr sass_function_get_cookie(IntPtr @cb)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_function_get_cookie(@cb)
                : SassExterns64.sass_function_get_cookie(@cb);
        }

        internal static IntPtr sass_compiler_get_last_import(IntPtr @compiler)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_compiler_get_last_import(@compiler)
                : SassExterns64.sass_compiler_get_last_import(@compiler);
        }

        internal static IntPtr sass_import_get_abs_path(IntPtr @entry)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_import_get_abs_path(@entry)
                : SassExterns64.sass_import_get_abs_path(@entry);
        }

        internal static void sass_importer_set_list_entry(IntPtr @list, int @idx, IntPtr @entry)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_importer_set_list_entry(@list, @idx, @entry);
            else
                SassExterns64.sass_importer_set_list_entry(@list, @idx, @entry);
        }

        internal static void sass_function_set_list_entry(IntPtr @list, int @pos, IntPtr @cb)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_function_set_list_entry(@list, @pos, @cb);
            else
                SassExterns64.sass_function_set_list_entry(@list, @pos, @cb);
        }

        internal static IntPtr sass_function_get_signature(IntPtr cb)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_function_get_signature(@cb)
                : SassExterns64.sass_function_get_signature(@cb);
        }

        internal static void sass_import_set_list_entry(IntPtr @list, int @idx, IntPtr @entry)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_import_set_list_entry(@list, @idx, @entry);
            else
                SassExterns64.sass_import_set_list_entry(@list, @idx, @entry);
        }

        internal static IntPtr sass_import_set_error(IntPtr @import, IntPtr @message, int @line, int @col)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_import_set_error(@import, @message, @line, @col)
                : SassExterns64.sass_import_set_error(@import, @message, @line, @col);
        }

        internal static IntPtr sass_make_error(SassSafeStringHandle @msg)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_error(@msg)
                : SassExterns64.sass_make_error(@msg);
        }

        internal static IntPtr sass_error_get_message(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_error_get_message(@value)
                : SassExterns64.sass_error_get_message(@value);
        }

        internal static IntPtr sass_make_warning(SassSafeStringHandle @msg)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_make_warning(@msg)
                : SassExterns64.sass_make_warning(@msg);
        }

        internal static IntPtr sass_warning_get_message(IntPtr @value)
        {
            return IntPtr.Size == 4
                ? SassExterns32.sass_warning_get_message(@value)
                : SassExterns64.sass_warning_get_message(@value);
        }

        // options
        internal static void sass_option_set_input_path(IntPtr @sass_options /*options*/, SassSafeStringHandle @input_path)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_input_path(@sass_options, @input_path);
            else
                SassExterns64.sass_option_set_input_path(@sass_options, @input_path);
        }

        internal static void sass_option_set_output_path(IntPtr @sass_options /*options*/, SassSafeStringHandle @output_path)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_output_path(@sass_options, @output_path);
            else
                SassExterns64.sass_option_set_output_path(@sass_options, @output_path);
        }

        internal static void sass_option_set_output_style(IntPtr @sass_options /*options*/, SassOutputStyle @output_style)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_output_style(@sass_options, @output_style);
            else
                SassExterns64.sass_option_set_output_style(@sass_options, @output_style);
        }

        internal static void sass_option_set_is_indented_syntax_src(IntPtr @sass_options /*options*/, bool @indented_syntax)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_is_indented_syntax_src(@sass_options, @indented_syntax);
            else
                SassExterns64.sass_option_set_is_indented_syntax_src(@sass_options, @indented_syntax);
        }

        internal static void sass_option_set_source_comments(IntPtr @sass_options /*options*/, bool @source_comments)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_source_comments(@sass_options, @source_comments);
            else
                SassExterns64.sass_option_set_source_comments(@sass_options, @source_comments);
        }

        internal static void sass_option_set_omit_source_map_url(IntPtr @sass_options /*options*/, bool @omit_source_map_url)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_omit_source_map_url(@sass_options, @omit_source_map_url);
            else
                SassExterns64.sass_option_set_omit_source_map_url(@sass_options, @omit_source_map_url);
        }

        internal static void sass_option_set_source_map_embed(IntPtr @sass_options /*options*/, bool @source_map_embed)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_source_map_embed(@sass_options, @source_map_embed);
            else
                SassExterns64.sass_option_set_source_map_embed(@sass_options, @source_map_embed);
        }

        internal static void sass_option_set_source_map_contents(IntPtr @sass_options /*options*/, bool @source_map_contents)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_source_map_contents(@sass_options, @source_map_contents);
            else
                SassExterns64.sass_option_set_source_map_contents(@sass_options, @source_map_contents);
        }

        internal static void sass_option_set_source_map_file(IntPtr @sass_options /*options*/, SassSafeStringHandle @source_map_file)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_source_map_file(@sass_options, @source_map_file);
            else
                SassExterns64.sass_option_set_source_map_file(@sass_options, @source_map_file);
        }

        internal static void sass_option_set_source_map_root(IntPtr @sass_options /*options*/, SassSafeStringHandle @source_map_root)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_source_map_root(@sass_options, @source_map_root);
            else
                SassExterns64.sass_option_set_source_map_root(@sass_options, @source_map_root);
        }

        internal static void sass_option_set_include_path(IntPtr @sass_options /*options*/, SassSafeStringHandle @include_path)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_include_path(@sass_options, @include_path);
            else
                SassExterns64.sass_option_set_include_path(@sass_options, @include_path);
        }

        internal static void sass_option_set_precision(IntPtr @sass_options /*options*/, int @precision)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_precision(@sass_options, @precision);
            else
                SassExterns64.sass_option_set_precision(@sass_options, @precision);
        }

        internal static void sass_option_set_indent(IntPtr @sass_options /*options*/, SassSafeStringHandle @indent)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_indent(@sass_options, @indent);
            else
                SassExterns64.sass_option_set_indent(@sass_options, @indent);
        }

        internal static void sass_option_set_linefeed(IntPtr @sass_options /*options*/, SassSafeStringHandle @linefeed)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_linefeed(@sass_options, @linefeed);
            else
                SassExterns64.sass_option_set_linefeed(@sass_options, @linefeed);
        }

        internal static void sass_option_set_c_importers(IntPtr @sass_options /*options*/, IntPtr @c_importers)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_c_importers(@sass_options, @c_importers);
            else
                SassExterns64.sass_option_set_c_importers(@sass_options, @c_importers);
        }

        internal static void sass_option_set_c_headers(IntPtr @sass_options /*options*/, IntPtr @c_headers)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_c_headers(@sass_options, @c_headers);
            else
                SassExterns64.sass_option_set_c_headers(@sass_options, @c_headers);
        }

        internal static void sass_option_set_c_functions(IntPtr @sass_options /*options*/, IntPtr @c_functions)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_set_c_functions(@sass_options, @c_functions);
            else
                SassExterns64.sass_option_set_c_functions(@sass_options, @c_functions);
        }

        internal static void sass_option_push_include_path(IntPtr @sass_options /*options*/, SassSafeStringHandle @path)
        {
            if (IntPtr.Size == 4)
                SassExterns32.sass_option_push_include_path(@sass_options, @path);
            else
                SassExterns64.sass_option_push_include_path(@sass_options, @path);
        }

        internal static IntPtr libsass_version()
        {
            return IntPtr.Size == 4
                ? SassExterns32.libsass_version()
                : SassExterns64.libsass_version();
        }

        internal static IntPtr libsass_language_version()
        {
            return IntPtr.Size == 4
                ? SassExterns32.libsass_language_version()
                : SassExterns64.libsass_language_version();
        }
    }
}
